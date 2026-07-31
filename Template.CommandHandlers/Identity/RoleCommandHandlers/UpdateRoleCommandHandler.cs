using Serilog;
using Serilog.Context;
using Template.Commands.Identity.RoleCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;
using Template.Events.Identity;

namespace Template.CommandHandlers.Identity.RoleCommandHandlers
{
    public class UpdateRoleCommandHandler : ICommandHandler<UpdateRoleCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public UpdateRoleCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(UpdateRoleCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling UpdateRoleCommand for RoleId: {RoleId}", command.RoleId);

                var role = await _dbContext.Roles.FindAsync(command.RoleId);
                if (role == null)
                {
                    Log.Warning("Role with RoleId: {RoleId} not found", command.RoleId);
                    return false;
                }

                if (!string.IsNullOrEmpty(command?.RoleName))
                {
                    Log.Information("Updating RoleName for RoleId: {RoleId}", command.RoleId);
                    role.RoleName = command.RoleName;
                }

                if (command?.Description != null)
                {
                    Log.Information("Updating Description for RoleId: {RoleId}", command.RoleId);
                    role.Description = command.Description;
                }

                if (command?.IsActive != null)
                {
                    Log.Information("Updating IsActive flag for RoleId: {RoleId}", command.RoleId);
                    role.IsActive = command.IsActive.Value;
                }

                role.UpdatedOn = DateTime.UtcNow;
                _dbContext.Roles.Update(role);
                await _dbContext.SaveChangesAsync();
                Log.Information("Role updated successfully for RoleId: {RoleId}", command.RoleId);

                var roleUpdatedEvent = new RoleUpdatedEvent
                {
                    TraceId = command.TraceId,
                    RoleId = role.RoleId,
                    RoleName = role.RoleName ?? string.Empty,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    UpdatedOn = role.UpdatedOn
                };

                await _bus.Publish(roleUpdatedEvent);
                Log.Information("RoleUpdatedEvent published for RoleId: {RoleId}", command.RoleId);

                return true;
            }
        }
    }
}
