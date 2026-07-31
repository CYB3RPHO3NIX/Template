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
    public class AssignPermissionToUserCommandHandler : ICommandHandler<AssignPermissionToUserCommand, Guid?>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public AssignPermissionToUserCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<Guid?> Handle(AssignPermissionToUserCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling AssignPermissionToUserCommand for UserId: {UserId}, PermissionId: {PermissionId}",
                    command.UserId, command.PermissionId);

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == command.UserId);
                if (user == null)
                {
                    Log.Warning("User with UserId: {UserId} not found", command.UserId);
                    return null;
                }

                var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.PermissionId == command.PermissionId);
                if (permission == null)
                {
                    Log.Warning("Permission with PermissionId: {PermissionId} not found", command.PermissionId);
                    return null;
                }

                var existingUserPermission = await _dbContext.UserPermissions
                    .FirstOrDefaultAsync(up => up.UserId == command.UserId && up.PermissionId == command.PermissionId);

                if (existingUserPermission != null)
                {
                    Log.Information("Permission already assigned to user - updating for UserId: {UserId}, PermissionId: {PermissionId}",
                        command.UserId, command.PermissionId);
                    existingUserPermission.IsAllowed = command.IsAllowed;
                    existingUserPermission.Reason = command.Reason;
                    existingUserPermission.ExpiresAt = command.ExpiresAt;
                    _dbContext.UserPermissions.Update(existingUserPermission);
                    await _dbContext.SaveChangesAsync();
                    return existingUserPermission.UserPermissionId;
                }

                var userPermissionId = Guid.NewGuid();
                var userPermission = new UserPermission
                {
                    UserPermissionId = userPermissionId,
                    UserId = command.UserId,
                    PermissionId = command.PermissionId,
                    IsAllowed = command.IsAllowed,
                    Reason = command.Reason,
                    AssignedAt = DateTime.UtcNow,
                    ExpiresAt = command.ExpiresAt
                };

                await _dbContext.UserPermissions.AddAsync(userPermission);
                await _dbContext.SaveChangesAsync();
                Log.Information("Permission assigned to user successfully with UserPermissionId: {UserPermissionId}", userPermissionId);

                var permissionAssignedEvent = new PermissionAssignedToUserEvent
                {
                    TraceId = command.TraceId,
                    UserPermissionId = userPermissionId,
                    UserId = command.UserId,
                    PermissionId = command.PermissionId,
                    PermissionName = permission.PermissionName,
                    PermissionCode = permission.PermissionCode,
                    IsAllowed = userPermission.IsAllowed,
                    Reason = userPermission.Reason,
                    AssignedAt = userPermission.AssignedAt,
                    ExpiresAt = userPermission.ExpiresAt
                };

                await _bus.Publish(permissionAssignedEvent);
                Log.Information("PermissionAssignedToUserEvent published for UserPermissionId: {UserPermissionId}", userPermissionId);

                return userPermissionId;
            }
        }
    }
}
