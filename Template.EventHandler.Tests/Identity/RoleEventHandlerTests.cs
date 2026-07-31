using Xunit;
using Moq;
using Template.EventHandlers.Identity;
using Template.Events.Identity;

namespace Template.EventHandler.Tests.Identity
{
    public class RoleEventHandlerTests
    {
        [Fact]
        public async Task RoleCreatedEventHandler_WithValidEvent_CompletesSucessfully()
        {
            var roleEvent = new RoleCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                RoleName = "Admin",
                Description = "Administrator role",
                CreatedOn = DateTime.UtcNow
            };

            var handler = new RoleCreatedEventHandler();

            await handler.Handle(roleEvent);
        }

        [Fact]
        public async Task RoleUpdatedEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var roleEvent = new RoleUpdatedEvent
            {
                TraceId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                RoleName = "Manager",
                Description = "Manager role",
                UpdatedOn = DateTime.UtcNow
            };

            var handler = new RoleUpdatedEventHandler();

            await handler.Handle(roleEvent);
        }

        [Fact]
        public async Task RoleDeletedEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var roleEvent = new RoleDeletedEvent
            {
                TraceId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                RoleName = "Deleted Role"
            };

            var handler = new RoleDeletedEventHandler();

            await handler.Handle(roleEvent);
        }

        [Fact]
        public async Task RoleAssignedToUserEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var roleEvent = new RoleAssignedToUserEvent
            {
                TraceId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RoleName = "Admin",
                AssignedAt = DateTime.UtcNow
            };

            var handler = new RoleAssignedToUserEventHandler();

            await handler.Handle(roleEvent);
        }

        [Fact]
        public async Task RoleRemovedFromUserEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var roleEvent = new RoleRemovedFromUserEvent
            {
                TraceId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                RoleName = "Admin"
            };

            var handler = new RoleRemovedFromUserEventHandler();

            await handler.Handle(roleEvent);
        }
    }
}
