using Xunit;
using Moq;
using Template.EventHandlers.Identity;
using Template.Events.Identity;
using Template.Contracts.EventHandler;

namespace Template.EventHandler.Tests.Identity
{
    public class UserCreatedEventHandlerTests
    {
        private readonly UserCreatedEventHandler _handler;

        public UserCreatedEventHandlerTests()
        {
            _handler = new UserCreatedEventHandler();
        }

        [Fact]
        public async Task Handle_WithValidEvent_CompletesSuccessfully()
        {
            // Arrange
            var @event = new UserCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "newuser",
                Email = "new@example.com",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(@event));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Handle_WithNullEvent_ThrowsException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(null));
        }

        [Fact]
        public async Task Handle_PreservesTraceId()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            var @event = new UserCreatedEvent
            {
                TraceId = traceId,
                UserId = Guid.NewGuid(),
                UserName = "user",
                Email = "user@example.com",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(@event));

            // Assert
            Assert.Null(exception);
            Assert.Equal(traceId, @event.TraceId);
        }

        [Fact]
        public async Task Handle_WithDifferentUsers_ProcessesEach()
        {
            // Arrange
            var event1 = new UserCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "user1",
                Email = "user1@example.com",
                CreatedOn = DateTime.UtcNow
            };

            var event2 = new UserCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "user2",
                Email = "user2@example.com",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var exception1 = await Record.ExceptionAsync(() => _handler.Handle(event1));
            var exception2 = await Record.ExceptionAsync(() => _handler.Handle(event2));

            // Assert
            Assert.Null(exception1);
            Assert.Null(exception2);
        }

        [Fact]
        public async Task Handle_WithEmptyUserName_StillProcesses()
        {
            // Arrange
            var @event = new UserCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = string.Empty,
                Email = "empty@example.com",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(@event));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Handle_WithEmptyEmail_StillProcesses()
        {
            // Arrange
            var @event = new UserCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "user",
                Email = string.Empty,
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(@event));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Handle_ImplementsIEventHandler()
        {
            // Assert
            Assert.IsAssignableFrom<IEventHandler<UserCreatedEvent>>(_handler);
        }

        [Fact]
        public async Task Handle_ReturnsCompletedTask()
        {
            // Arrange
            var @event = new UserCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "user",
                Email = "user@example.com",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            var task = _handler.Handle(@event);

            // Assert
            Assert.IsAssignableFrom<Task>(task);
            await task;
            Assert.True(task.IsCompleted);
        }

        [Fact]
        public async Task Handle_CanBeCalledMultipleTimes()
        {
            // Arrange
            var @event = new UserCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "user",
                Email = "user@example.com",
                CreatedOn = DateTime.UtcNow
            };

            // Act
            await _handler.Handle(@event);
            await _handler.Handle(@event);
            await _handler.Handle(@event);

            // Assert - No exceptions thrown
        }

        [Fact]
        public async Task Handle_WithOldCreatedDate_StillProcesses()
        {
            // Arrange
            var @event = new UserCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "user",
                Email = "user@example.com",
                CreatedOn = DateTime.UtcNow.AddYears(-1)
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(@event));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task Handle_WithFutureCreatedDate_StillProcesses()
        {
            // Arrange
            var @event = new UserCreatedEvent
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "user",
                Email = "user@example.com",
                CreatedOn = DateTime.UtcNow.AddYears(1)
            };

            // Act
            var exception = await Record.ExceptionAsync(() => _handler.Handle(@event));

            // Assert
            Assert.Null(exception);
        }
    }
}
