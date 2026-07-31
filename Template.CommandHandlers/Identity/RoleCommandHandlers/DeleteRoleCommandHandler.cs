using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Commands.Identity.RoleCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;
using Template.Events.Identity;

namespace Template.CommandHandlers.Identity.RoleCommandHandlers
{
    public class DeleteRoleCommandHandler : ICommandHandler<DeleteRoleCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public DeleteRoleCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(DeleteRoleCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling DeleteRoleCommand for RoleId: {RoleId}", command.RoleId);

                var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == command.RoleId);
                if (role == null)
                {
                    Log.Warning("Role with RoleId: {RoleId} does not exist", command.RoleId);
                    return false;
                }

                if (role.IsSystem)
                {
                    Log.Warning("Cannot delete system role with RoleId: {RoleId}", command.RoleId);
                    return false;
                }

                _dbContext.Roles.Remove(role);
                await _dbContext.SaveChangesAsync();
                Log.Information("Role with RoleId: {RoleId} has been deleted", command.RoleId);

                var roleDeletedEvent = new RoleDeletedEvent
                {
                    TraceId = command.TraceId,
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    DeletedOn = DateTime.UtcNow
                };

                await _bus.Publish(roleDeletedEvent);
                Log.Information("RoleDeletedEvent published for RoleId: {RoleId}", command.RoleId);

                return true;
            }
        }
    }
}
