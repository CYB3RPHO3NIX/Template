using Serilog;
using Serilog.Context;
using Template.Commands.Identity.PermissionCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;
using Template.Events.Identity;

namespace Template.CommandHandlers.Identity.PermissionCommandHandlers
{
    public class UpdatePermissionCommandHandler : ICommandHandler<UpdatePermissionCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public UpdatePermissionCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdatePermissionCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling UpdatePermissionCommand for PermissionId: {PermissionId}", command.PermissionId);

                var permission = await _dbContext.Permissions.FindAsync(command.PermissionId);
                if (permission == null)
                {
                    Log.Warning("Permission with PermissionId: {PermissionId} not found", command.PermissionId);
                    return false;
                }

                if (!string.IsNullOrEmpty(command?.PermissionName))
                {
                    Log.Information("Updating PermissionName for PermissionId: {PermissionId}", command.PermissionId);
                    permission.PermissionName = command.PermissionName;
                }

                if (command?.Description != null)
                {
                    Log.Information("Updating Description for PermissionId: {PermissionId}", command.PermissionId);
                    permission.Description = command.Description;
                }

                if (command?.Category != null)
                {
                    Log.Information("Updating Category for PermissionId: {PermissionId}", command.PermissionId);
                    permission.Category = command.Category;
                }

                if (command?.IsActive != null)
                {
                    Log.Information("Updating IsActive flag for PermissionId: {PermissionId}", command.PermissionId);
                    permission.IsActive = command.IsActive.Value;
                }

                permission.UpdatedOn = DateTime.UtcNow;
                _dbContext.Permissions.Update(permission);
                await _dbContext.SaveChangesAsync();
                Log.Information("Permission updated successfully for PermissionId: {PermissionId}", command.PermissionId);

                var permissionUpdatedEvent = new PermissionUpdatedEvent
                {
                    TraceId = command.TraceId,
                    PermissionId = permission.PermissionId,
                    PermissionName = permission.PermissionName ?? string.Empty,
                    Description = permission.Description,
                    Category = permission.Category,
                    IsActive = permission.IsActive,
                    UpdatedOn = permission.UpdatedOn
                };

                await _bus.Publish(permissionUpdatedEvent);
                Log.Information("PermissionUpdatedEvent published for PermissionId: {PermissionId}", command.PermissionId);

                return true;
            }
        }
    }
}
