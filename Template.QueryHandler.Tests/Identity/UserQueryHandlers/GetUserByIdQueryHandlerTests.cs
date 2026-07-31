using Xunit;
using Template.QueryHandlers.Identity.UserQueryHandlers;
using Template.Queries.Identity.UserQueries;
using Template.Database.Domain.Contexts;
using Template.Database.Domain.Entities;
using Template.Shared.Models.DTOs.Identity;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace Template.QueryHandler.Tests.Identity.UserQueryHandlers
{
    public class GetUserByIdQueryHandlerTests : IDisposable
    {
        private readonly TemplateDbContext _dbContext;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<TemplateDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TemplateDbContext(options);

            var config = TypeAdapterConfig.GlobalSettings;
            config.NewConfig<User, UserDTO>()
                .Map(dest => dest.Id, src => src.UserId)
                .Map(dest => dest.UserName, src => src.Username);

            _handler = new GetUserByIdQueryHandler(_dbContext);
        }

        [Fact]
        public async Task Handle_WithExistingUser_ReturnsUserDTO()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var query = new GetUserByIdQuery
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            };

            // Act
            var result = await _handler.Handle(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("testuser", result.UserName);
            Assert.Equal("test@example.com", result.Email);
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ReturnsNull()
        {
            // Arrange
            var query = new GetUserByIdQuery
            {
                TraceId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            // Act
            var result = await _handler.Handle(query);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_WithInactiveUser_ReturnsUserDTO()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "inactiveuser",
                Email = "inactive@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = false
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var query = new GetUserByIdQuery
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            };

            // Act
            var result = await _handler.Handle(query);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsActive);
        }

        [Fact]
        public async Task Handle_MapsDTOCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var username = "mappingtest";
            var email = "mapping@example.com";

            var user = new User
            {
                UserId = userId,
                Username = username,
                Email = email,
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var query = new GetUserByIdQuery
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            };

            // Act
            var result = await _handler.Handle(query);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<UserDTO>(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(username, result.UserName);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task Handle_WithMultipleUsers_ReturnsCorrectUser()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var userId3 = Guid.NewGuid();

            await _dbContext.Users.AddRangeAsync(
                new User
                {
                    UserId = userId1,
                    Username = "user1",
                    Email = "user1@example.com",
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    IsActive = true
                },
                new User
                {
                    UserId = userId2,
                    Username = "user2",
                    Email = "user2@example.com",
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    IsActive = true
                },
                new User
                {
                    UserId = userId3,
                    Username = "user3",
                    Email = "user3@example.com",
                    PasswordHash = "hash",
                    PasswordSalt = "salt",
                    IsActive = true
                }
            );

            await _dbContext.SaveChangesAsync();

            var query = new GetUserByIdQuery
            {
                TraceId = Guid.NewGuid(),
                UserId = userId2
            };

            // Act
            var result = await _handler.Handle(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("user2", result.UserName);
        }

        [Fact]
        public async Task Handle_DoesNotModifyDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                UserId = userId,
                Username = "readonly",
                Email = "readonly@example.com",
                PasswordHash = "hash",
                PasswordSalt = "salt",
                IsActive = true
            };

            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var query = new GetUserByIdQuery
            {
                TraceId = Guid.NewGuid(),
                UserId = userId
            };

            var userCountBefore = await _dbContext.Users.CountAsync();

            // Act
            await _handler.Handle(query);

            // Assert
            var userCountAfter = await _dbContext.Users.CountAsync();
            Assert.Equal(userCountBefore, userCountAfter);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }
    }
}
