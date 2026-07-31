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
    public class UpdateUserCommandHandlerTests : IDisposable
    {
        private readonly TemplateDbContext _dbContext;
        private readonly Mock<IServiceBus> _mockBus;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<TemplateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TemplateDbContext(options);
            _mockBus = new Mock<IServiceBus>();
            _handler = new UpdateUserCommandHandler(_mockBus.Object, _dbContext);
        }

        [Fact]
        public async Task Handle_WithValidCommand_UpdatesUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "oldusername",
                Email = "old@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var command = new UpdateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId,
                UserName = "newusername",
                Email = "new@example.com",
                Password = "NewPassword123",
                IsActive = false
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result);

            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            Assert.NotNull(updatedUser);
            Assert.Equal("newusername", updatedUser.Username);
            Assert.Equal("new@example.com", updatedUser.Email);
            Assert.False(updatedUser.IsActive);
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ReturnsFalse()
        {
            // Arrange
            var command = new UpdateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                UserName = "nonexistent",
                Email = "nonexistent@example.com",
                Password = "Password123",
                IsActive = true
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Handle_UpdatesPasswordWithNewSalt()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var originalHash = "originalhash";
            var user = new User
            {
                UserId = userId,
                Username = "user",
                Email = "user@example.com",
                PasswordHash = originalHash,
                PasswordSalt = "originalsalt",
                IsActive = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var command = new UpdateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId,
                UserName = "user",
                Email = "user@example.com",
                Password = "NewPassword123",
                IsActive = true
            };

            // Act
            var result = await _handler.Handle(command);

            // Assert
            Assert.True(result);

            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            Assert.NotEqual(originalHash, updatedUser.PasswordHash);
        }

        [Fact]
        public async Task Handle_PreservesCreatedByAuditFields()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var createdBy = Guid.NewGuid();
            var createdDate = DateTime.UtcNow.AddHours(-1);

            var user = new User
            {
                UserId = userId,
                Username = "user",
                Email = "user@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = true,
                CreatedBy = createdBy,
                CreatedOn = createdDate
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var command = new UpdateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId,
                UserName = "updated",
                Email = "updated@example.com",
                Password = "Password123",
                IsActive = true
            };

            // Act
            await _handler.Handle(command);

            // Assert
            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            Assert.Equal(createdBy, updatedUser.CreatedBy);
            Assert.Equal(createdDate, updatedUser.CreatedOn);
        }

        [Fact]
        public async Task Handle_UpdatesAuditTrail()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "user",
                Email = "user@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = true,
                UpdatedOn = DateTime.MinValue
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var command = new UpdateUserCommand
            {
                TraceId = Guid.NewGuid(),
                UserId = userId,
                UserName = "updated",
                Email = "updated@example.com",
                Password = "Password123",
                IsActive = true
            };

            // Act
            var beforeUpdate = DateTime.UtcNow;
            await _handler.Handle(command);
            var afterUpdate = DateTime.UtcNow;

            // Assert
            var updatedUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            Assert.True(updatedUser.UpdatedOn >= beforeUpdate && updatedUser.UpdatedOn <= afterUpdate);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
