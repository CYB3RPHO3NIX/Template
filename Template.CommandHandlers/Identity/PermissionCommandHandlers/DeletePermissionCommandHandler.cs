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
    public class DeletePermissionCommandHandler : ICommandHandler<DeletePermissionCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public DeletePermissionCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeletePermissionCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling DeletePermissionCommand for PermissionId: {PermissionId}", command.PermissionId);

                var permission = await _dbContext.Permissions.FirstOrDefaultAsync(p => p.PermissionId == command.PermissionId);
                if (permission == null)
                {
                    Log.Warning("Permission with PermissionId: {PermissionId} does not exist", command.PermissionId);
                    return false;
                }

                _dbContext.Permissions.Remove(permission);
                await _dbContext.SaveChangesAsync();
                Log.Information("Permission with PermissionId: {PermissionId} has been deleted", command.PermissionId);

                var permissionDeletedEvent = new PermissionDeletedEvent
                {
                    TraceId = command.TraceId,
                    PermissionId = permission.PermissionId,
                    PermissionName = permission.PermissionName,
                    PermissionCode = permission.PermissionCode,
                    DeletedOn = DateTime.UtcNow
                };

                await _bus.Publish(permissionDeletedEvent);
                Log.Information("PermissionDeletedEvent published for PermissionId: {PermissionId}", command.PermissionId);

                return true;
            }
        }
    }
}
