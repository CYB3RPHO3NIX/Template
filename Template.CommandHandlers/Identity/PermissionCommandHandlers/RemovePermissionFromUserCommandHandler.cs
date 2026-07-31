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
    public class RemovePermissionFromUserCommandHandler : ICommandHandler<RemovePermissionFromUserCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public RemovePermissionFromUserCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(RemovePermissionFromUserCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling RemovePermissionFromUserCommand for UserId: {UserId}, PermissionId: {PermissionId}",
                    command.UserId, command.PermissionId);

                var userPermission = await _dbContext.UserPermissions
                    .FirstOrDefaultAsync(up => up.UserId == command.UserId && up.PermissionId == command.PermissionId);

                if (userPermission == null)
                {
                    Log.Warning("User permission not found for UserId: {UserId}, PermissionId: {PermissionId}",
                        command.UserId, command.PermissionId);
                    return false;
                }

                var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.PermissionId == command.PermissionId);
                var permissionName = permission?.PermissionName ?? "Unknown";
                var permissionCode = permission?.PermissionCode ?? "Unknown";

                _dbContext.UserPermissions.Remove(userPermission);
                await _dbContext.SaveChangesAsync();
                Log.Information("Permission removed from user successfully for UserId: {UserId}, PermissionId: {PermissionId}",
                    command.UserId, command.PermissionId);

                var permissionRemovedEvent = new PermissionRemovedFromUserEvent
                {
                    TraceId = command.TraceId,
                    UserId = command.UserId,
                    PermissionId = command.PermissionId,
                    PermissionName = permissionName,
                    PermissionCode = permissionCode,
                    RemovedAt = DateTime.UtcNow
                };

                await _bus.Publish(permissionRemovedEvent);
                Log.Information("PermissionRemovedFromUserEvent published for UserId: {UserId}, PermissionId: {PermissionId}",
                    command.UserId, command.PermissionId);

                return true;
            }
        }
    }
}
