using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Commands.Identity.PermissionCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;
using Template.Database.Domain.Entities;
using Template.Events.Identity;

namespace Template.CommandHandlers.Identity.PermissionCommandHandlers
{
    public class AssignPermissionToRoleCommandHandler : ICommandHandler<AssignPermissionToRoleCommand, Guid?>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public AssignPermissionToRoleCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<Guid?> Handle(AssignPermissionToRoleCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling AssignPermissionToRoleCommand for RoleId: {RoleId}, PermissionId: {PermissionId}",
                    command.RoleId, command.PermissionId);

                var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == command.RoleId);
                if (role == null)
                {
                    Log.Warning("Role with RoleId: {RoleId} not found", command.RoleId);
                    return null;
                }

                var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.PermissionId == command.PermissionId);
                if (permission == null)
                {
                    Log.Warning("Permission with PermissionId: {PermissionId} not found", command.PermissionId);
                    return null;
                }

                var existingRolePermission = await _dbContext.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == command.RoleId && rp.PermissionId == command.PermissionId);

                if (existingRolePermission != null)
                {
                    Log.Information("Permission already assigned to role - updating IsAllowed for RoleId: {RoleId}, PermissionId: {PermissionId}",
                        command.RoleId, command.PermissionId);
                    existingRolePermission.IsAllowed = command.IsAllowed;
                    _dbContext.RolePermissions.Update(existingRolePermission);
                    await _dbContext.SaveChangesAsync();
                    return existingRolePermission.RolePermissionId;
                }

                var rolePermissionId = Guid.NewGuid();
                var rolePermission = new RolePermission
                {
                    RolePermissionId = rolePermissionId,
                    RoleId = command.RoleId,
                    PermissionId = command.PermissionId,
                    IsAllowed = command.IsAllowed,
                    AssignedAt = DateTime.UtcNow
                };

                await _dbContext.RolePermissions.AddAsync(rolePermission);
                await _dbContext.SaveChangesAsync();
                Log.Information("Permission assigned to role successfully with RolePermissionId: {RolePermissionId}", rolePermissionId);

                var permissionAssignedEvent = new PermissionAssignedToRoleEvent
                {
                    TraceId = command.TraceId,
                    RolePermissionId = rolePermissionId,
                    RoleId = command.RoleId,
                    PermissionId = command.PermissionId,
                    PermissionName = permission.PermissionName,
                    PermissionCode = permission.PermissionCode,
                    IsAllowed = rolePermission.IsAllowed,
                    AssignedAt = rolePermission.AssignedAt
                };

                await _bus.Publish(permissionAssignedEvent);
                Log.Information("PermissionAssignedToRoleEvent published for RolePermissionId: {RolePermissionId}", rolePermissionId);

                return rolePermissionId;
            }
        }
    }
}
