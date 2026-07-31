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
    public class CreatePermissionCommandHandler : ICommandHandler<CreatePermissionCommand, Guid?>
    {
        private readonly IServiceBus _bus;
        private readonly TemplateDbContext _dbContext;

        public CreatePermissionCommandHandler(IServiceBus bus, TemplateDbContext dbContext)
        {
            _bus = bus;
            _dbContext = dbContext;
        }

        public async Task<Guid?> Handle(CreatePermissionCommand command)
        {
            using (LogContext.PushProperty("TraceId", command.TraceId))
            {
                Log.Information("Handling CreatePermissionCommand for PermissionName: {PermissionName}, PermissionCode: {PermissionCode}",
                    command.PermissionName, command.PermissionCode);

                var permissionId = Guid.NewGuid();
                var permission = new Permission
                {
                    PermissionId = permissionId,
                    PermissionName = command.PermissionName,
                    PermissionCode = command.PermissionCode,
                    Description = command.Description,
                    Category = command.Category,
                    ParentPermissionId = command.ParentPermissionId,
                    IsActive = true,
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };

                await _dbContext.Permissions.AddAsync(permission);
                await _dbContext.SaveChangesAsync();
                Log.Information("Permission created successfully with PermissionId: {PermissionId}", permissionId);

                var permissionCreatedEvent = new PermissionCreatedEvent
                {
                    TraceId = command.TraceId,
                    PermissionId = permissionId,
                    PermissionName = permission.PermissionName,
                    PermissionCode = permission.PermissionCode,
                    Description = permission.Description,
                    Category = permission.Category,
                    ParentPermissionId = permission.ParentPermissionId,
                    CreatedOn = permission.CreatedOn
                };

                await _bus.Publish(permissionCreatedEvent);
                Log.Information("PermissionCreatedEvent published for PermissionId: {PermissionId}", permissionId);

                return permissionId;
            }
        }
    }
}
