using Xunit;
using Template.EventHandlers.Identity;
using Template.Events.Identity;

namespace Template.EventHandler.Tests.Identity
{
    public class PermissionEventHandlerTests
    {
        [Fact]
        public async Task PermissionCreatedEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var permEvent = new PermissionCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                PermissionName = "Create Users",
                PermissionCode = "USER_CREATE",
                CreatedOn = DateTime.UtcNow
            };

            var handler = new PermissionCreatedEventHandler();

            await handler.Handle(permEvent);
        }

        [Fact]
        public async Task PermissionUpdatedEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var permEvent = new PermissionUpdatedEvent
            {
                TraceId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                PermissionName = "Delete Users",
                Category = "Users",
                IsActive = true,
                UpdatedOn = DateTime.UtcNow
            };

            var handler = new PermissionUpdatedEventHandler();

            await handler.Handle(permEvent);
        }

        [Fact]
        public async Task PermissionDeletedEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var permEvent = new PermissionDeletedEvent
            {
                TraceId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                PermissionName = "Deleted Permission"
            };

            var handler = new PermissionDeletedEventHandler();

            await handler.Handle(permEvent);
        }

        [Fact]
        public async Task PermissionAssignedToRoleEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var permEvent = new PermissionAssignedToRoleEvent
            {
                TraceId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                PermissionName = "Create Users",
                IsAllowed = true,
                AssignedAt = DateTime.UtcNow
            };

            var handler = new PermissionAssignedToRoleEventHandler();

            await handler.Handle(permEvent);
        }

        [Fact]
        public async Task PermissionRemovedFromRoleEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var permEvent = new PermissionRemovedFromRoleEvent
            {
                TraceId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                RoleId = Guid.NewGuid(),
                PermissionName = "Create Users"
            };

            var handler = new PermissionRemovedFromRoleEventHandler();

            await handler.Handle(permEvent);
        }

        [Fact]
        public async Task PermissionAssignedToUserEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var permEvent = new PermissionAssignedToUserEvent
            {
                TraceId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PermissionName = "Create Users",
                IsAllowed = true,
                AssignedAt = DateTime.UtcNow
            };

            var handler = new PermissionAssignedToUserEventHandler();

            await handler.Handle(permEvent);
        }

        [Fact]
        public async Task PermissionRemovedFromUserEventHandler_WithValidEvent_CompletesSuccessfully()
        {
            var permEvent = new PermissionRemovedFromUserEvent
            {
                TraceId = Guid.NewGuid(),
                PermissionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                PermissionName = "Create Users"
            };

            var handler = new PermissionRemovedFromUserEventHandler();

            await handler.Handle(permEvent);
        }
    }
}
