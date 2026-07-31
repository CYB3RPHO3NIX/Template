using Moq;
using Xunit;
using Template.CommandHandlers.Identity.UserCommandHandlers;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.ServiceBus;
using Template.Database.Domain.Contexts;
using Template.Database.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Template.CommandHandler.Tests.Identity.UserCommandHandlers
{
    public class DeleteUserCommandHandlerTests : IDisposable
    {
        private readonly TemplateDbContext _dbContext;
        private readonly Mock<IServiceBus> _mockBus;
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<TemplateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TemplateDbContext(options);
            _mockBus = new Mock<IServiceBus>();
            _handler = new DeleteUserCommandHandler(_mockBus.Object, _dbContext);
        }

        [Fact]
        public async Task Handle_WithExistingUser_DeletesSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "usertodeleted",
                Email = "delete@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var command = new DeleteUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result);

            var deletedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            var command = new DeleteUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Handle_DoesNotDeleteOtherUsers()
        {
            // Arrange
            var userIdToDelete = Guid.NewGuid();
            var userIdToKeep1 = Guid.NewGuid();
            var userIdToKeep2 = Guid.NewGuid();

            await _dbContext.Users.AddRangeAsync(
                new User
                {
                    UserId = userIdToDelete,
                    Username = "todelete",
                    Email = "delete@example.com",
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    IsActive = true
                },
                new User
                {
                    UserId = userIdToKeep1,
                    Username = "keep1",
                    Email = "keep1@example.com",
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    IsActive = true
                },
                new User
                {
                    UserId = userIdToKeep2,
                    Username = "keep2",
                    Email = "keep2@example.com",
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    IsActive = true
                }
            );

            await _dbContext.SaveChangesAsync();

            var command = new DeleteUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userIdToDelete
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result);

            var user1 = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userIdToKeep1);
            var user2 = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userIdToKeep2);

            Assert.NotNull(user1);
            Assert.NotNull(user2);
        }

        [Fact]
        public async Task Handle_RemovesUserCompletely()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "complete",
                Email = "complete@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var userCountBefore = await _dbContext.Users.CountAsync();

            var command = new DeleteUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            };

            // Act
            await _handler.Handle(command);

            // Assert
            var userCountAfter = await _dbContext.Users.CountAsync();
            Assert.Equal(userCountBefore - 1, userCountAfter);
        }

        [Fact]
        public async Task Handle_WithInactiveUser_DeletesSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "inactive",
                Email = "inactive@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = false
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var command = new DeleteUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result);

            var deletedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            Assert.Null(deletedUser);
        }

        [Fact]
        public async Task Handle_CanBeCalledMultipleTimes()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "multi",
                Email = "multi@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var command = new DeleteUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            };

            // Act
            var result1 = await _handler.Handle(command);
            var result2 = await _handler.Handle(command);

            // Assert
            Assert.True(result1);  // First delete succeeds
            Assert.False(result2); // Second delete fails (user already deleted)
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
