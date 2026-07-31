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
    public class CreateRoleCommandHandler : ICommandHandler<CreateRoleCommand, Guid?>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public CreateRoleCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<Guid?> Handle(CreateRoleCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling CreateRoleCommand for RoleName: {RoleName}", command.RoleName);

                var roleId = Guid.NewGuid();
                var role = new Role
                {
                    RoleId = roleId,
                    RoleName = command.RoleName,
                    Description = command.Description,
                    IsSystem = command.IsSystem,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };

                await _dbContext.Roles.AddAsync(role);
                await _dbContext.SaveChangesAsync();
                Log.Information("Role created successfully with RoleId: {RoleId}", roleId);

                var roleCreatedEvent = new RoleCreatedEvent
                {
                    TraceId = command.TraceId,
                    RoleId = roleId,
                    RoleName = role.RoleName,
                    Description = role.Description,
                    IsSystem = role.IsSystem,
                    CreatedOn = role.CreatedOn
                };

                await _bus.Publish(roleCreatedEvent);
                Log.Information("RoleCreatedEvent published for RoleId: {RoleId}", roleId);

                return roleId;
            }
        }
    }
}
