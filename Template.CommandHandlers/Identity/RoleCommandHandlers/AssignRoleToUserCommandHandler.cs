using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Commands.Identity.RoleCommands;
using Template.Contracts.CommandHandler;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;
using Template.Database.Domain.Entities;
using Template.Events.Identity;

namespace Template.CommandHandlers.Identity.RoleCommandHandlers
{
    public class AssignRoleToUserCommandHandler : ICommandHandler<AssignRoleToUserCommand, Guid?>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public AssignRoleToUserCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<Guid?> Handle(AssignRoleToUserCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling AssignRoleToUserCommand for UserId: {UserId}, RoleId: {RoleId}",
                    command.UserId, command.RoleId);

                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == command.UserId);
                if (user == null)
                {
                    Log.Warning("User with UserId: {UserId} not found", command.UserId);
                    return null;
                }

                var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.RoleId == command.RoleId);
                if (role == null)
                {
                    Log.Warning("Role with RoleId: {RoleId} not found", command.RoleId);
                    return null;
                }

                var existingUserRole = await _dbContext.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == command.UserId && ur.RoleId == command.RoleId);
                if (existingUserRole != null)
                {
                    Log.Information("User already has role assigned - updating expiration for UserId: {UserId}, RoleId: {RoleId}",
                        command.UserId, command.RoleId);
                    existingUserRole.ExpiresAt = command.ExpiresAt;
                    _dbContext.UserRoles.Update(existingUserRole);
                    await _dbContext.SaveChangesAsync();
                    return existingUserRole.UserRoleId;
                }

                var userRoleId = Guid.NewGuid();
                var userRole = new UserRole
                {
                    UserRoleId = userRoleId,
                    UserId = command.UserId,
                    RoleId = command.RoleId,
                    AssignedAt = DateTime.UtcNow,
                    ExpiresAt = command.ExpiresAt
                };

                await _dbContext.UserRoles.AddAsync(userRole);
                await _dbContext.SaveChangesAsync();
                Log.Information("Role assigned to user successfully with UserRoleId: {UserRoleId}", userRoleId);

                var roleAssignedEvent = new RoleAssignedToUserEvent
                {
                    TraceId = command.TraceId,
                    UserRoleId = userRoleId,
                    UserId = command.UserId,
                    RoleId = command.RoleId,
                    RoleName = role.RoleName,
                    AssignedAt = userRole.AssignedAt,
                    ExpiresAt = userRole.ExpiresAt
                };

                await _bus.Publish(roleAssignedEvent);
                Log.Information("RoleAssignedToUserEvent published for UserRoleId: {UserRoleId}", userRoleId);

                return userRoleId;
            }
        }
    }
}
