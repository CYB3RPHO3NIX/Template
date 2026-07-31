using Moq;
using Xunit;
using Template.CommandHandlers.Identity.UserCommandHandlers;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;
using Template.Database.Domain.Entities;
using Template.Events.Identity;
using Template.Queries.Identity.UserQueries;
using Microsoft.EntityFrameworkCore;
using Template.Shared.Models.Common;

namespace Template.CommandHandler.Tests.Identity.UserCommandHandlers
{
    public class CreateUserCommandHandlerTests : IDisposable
    {
        private readonly TemplateDbContext _dbContext;
        private readonly Mock<IServiceBus> _mockBus;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<TemplateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TemplateDbContext(options);
            _mockBus = new Mock<IServiceBus>();
            _handler = new CreateUserCommandHandler(_mockBus.Object, _dbContext);
        }

        [Fact]
        public async Task Handle_WithValidCommand_CreatesUserSuccessfully()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                Password = "SecurePassword123"
            };

            _mockBus
                .Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result);

            var createdUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == result);
            Assert.NotNull(createdUser);
            Assert.Equal("testuser", createdUser.Username);
            Assert.Equal("test@example.com", createdUser.Email);
            Assert.True(createdUser.IsActive);
        }

        [Fact]
        public async Task Handle_WithExistingUser_ReturnsNull()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserName = "existinguser",
                Email = "existing@example.com",
                Password = "SecurePassword123"
            };

            _mockBus
                .Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.Null(result);

            // Verify user was not added to database
            var userCount = await _dbContext.Users.CountAsync();
            Assert.Equal(0, userCount);
        }

        [Fact]
        public async Task Handle_PublishesUserCreatedEvent()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserName = "newuser",
                Email = "new@example.com",
                Password = "SecurePassword123"
            };

            _mockBus
                .Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command);

            // Assert
            _mockBus.Verify(
                x => x.Publish(It.IsAny<UserCreatedEvent>()),
                Times.Once,
                "UserCreatedEvent should be published after user creation"
            );
        }

        [Fact]
        public async Task Handle_WithSameEmail_ReturnsNull()
        {
            // Arrange
            var email = "duplicate@example.com";
            var command = new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserName = "user2",
                Email = email,
                Password = "SecurePassword123"
            };

            _mockBus
                .Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_SavesPasswordWithSalt()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserName = "secureuser",
                Email = "secure@example.com",
                Password = "MySecurePassword123"
            };

            _mockBus
                .Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.NotNull(result);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == result);

            Assert.NotNull(user);
            Assert.NotEmpty(user.PasswordHash);
            Assert.NotEmpty(user.PasswordSalt);
            Assert.NotEqual("MySecurePassword123", user.PasswordHash); // Should be hashed
        }

        [Fact]
        public async Task Handle_ChecksUserExistenceBeforeCreation()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserName = "checkuser",
                Email = "check@example.com",
                Password = "SecurePassword123"
            };

            _mockBus
                .Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
                .ReturnsAsync(false);

            // Act
            await _handler.Handle(command);

            // Assert
            _mockBus.Verify(
                x => x.Send(It.Is<DoesUserExistQuery>(q =>
                    q.Email == "check@example.com" &&
                    q.UserName == "checkuser")),
                Times.Once,
                "Should check if user exists with provided email and username"
            );
        }

        [Fact]
        public async Task Handle_WithNullEmail_StoresEmptyString()
        {
            // Arrange
            var command = new CreateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserName = "noemailtuser",
                Email = null,
                Password = "SecurePassword123"
            };

            _mockBus
                .Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.NotNull(result);
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == result);
            Assert.Equal(string.Empty, user.Email);
        }

        [Fact]
        public async Task Handle_MaintainsDataConsistency()
        {
            // Arrange
            var traceId = Guid.NewGuid();
            var command = new CreateUserCommand
            {
                TraceId = traceId,
                UserName = "consistencyuser",
                Email = "consistency@example.com",
                Password = "SecurePassword123"
            };

            _mockBus
                .Setup(x => x.Send(It.IsAny<DoesUserExistQuery>()))
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command);

            // Assert - Verify event published with same TraceId
            _mockBus.Verify(
                x => x.Publish(It.Is<UserCreatedEvent>(e => e.TraceId == traceId)),
                Times.Once
            );
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
