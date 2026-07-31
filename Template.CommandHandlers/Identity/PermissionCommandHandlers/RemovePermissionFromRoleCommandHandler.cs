using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Commands.Identity.PermissionCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;
using Template.Events.Identity;

namespace Template.CommandHandlers.Identity.PermissionCommandHandlers
{
    public class RemovePermissionFromRoleCommandHandler : ICommandHandler<RemovePermissionFromRoleCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public RemovePermissionFromRoleCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(RemovePermissionFromRoleCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling RemovePermissionFromRoleCommand for RoleId: {RoleId}, PermissionId: {PermissionId}",
                    command.RoleId, command.PermissionId);

                var rolePermission = await _dbContext.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == command.RoleId && rp.PermissionId == command.PermissionId);

                if (rolePermission == null)
                {
                    Log.Warning("Role permission not found for RoleId: {RoleId}, PermissionId: {PermissionId}",
                        command.RoleId, command.PermissionId);
                    return false;
                }

                var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.PermissionId == command.PermissionId);
                var permissionName = permission?.PermissionName ?? "Unknown";
                var permissionCode = permission?.PermissionCode ?? "Unknown";

                _dbContext.RolePermissions.Remove(rolePermission);
                await _dbContext.SaveChangesAsync();
                Log.Information("Permission removed from role successfully for RoleId: {RoleId}, PermissionId: {PermissionId}",
                    command.RoleId, command.PermissionId);

                var permissionRemovedEvent = new PermissionRemovedFromRoleEvent
                {
                    TraceId = command.TraceId,
                    RoleId = command.RoleId,
                    PermissionId = command.PermissionId,
                    PermissionName = permissionName,
                    PermissionCode = permissionCode,
                    RemovedAt = DateTime.UtcNow
                };

                await _bus.Publish(permissionRemovedEvent);
                Log.Information("PermissionRemovedFromRoleEvent published for RoleId: {RoleId}, PermissionId: {PermissionId}",
                    command.RoleId, command.PermissionId);

                return true;
            }
        }
    }
}
