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
    public class RemoveRoleFromUserCommandHandler : ICommandHandler<RemoveRoleFromUserCommand, bool>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public RemoveRoleFromUserCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<bool> Handle(RemoveRoleFromUserCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling RemoveRoleFromUserCommand for UserId: {UserId}, RoleId: {RoleId}",
                    command.UserId, command.RoleId);

                var userRole = await _dbContext.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == command.UserId && ur.RoleId == command.RoleId);

                if (userRole == null)
                {
                    Log.Warning("User role not found for UserId: {UserId}, RoleId: {RoleId}",
                        command.UserId, command.RoleId);
                    return false;
                }

                var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == command.RoleId);
                var roleName = role?.RoleName ?? "Unknown";

                _dbContext.UserRoles.Remove(userRole);
                await _dbContext.SaveChangesAsync();
                Log.Information("Role removed from user successfully for UserId: {UserId}, RoleId: {RoleId}",
                    command.UserId, command.RoleId);

                var roleRemovedEvent = new RoleRemovedFromUserEvent
                {
                    TraceId = command.TraceId,
                    UserId = command.UserId,
                    RoleId = command.RoleId,
                    RoleName = roleName,
                    RemovedAt = DateTime.UtcNow
                };

                await _bus.Publish(roleRemovedEvent);
                Log.Information("RoleRemovedFromUserEvent published for UserId: {UserId}, RoleId: {RoleId}",
                    command.UserId, command.RoleId);

                return true;
            }
        }
    }
}
