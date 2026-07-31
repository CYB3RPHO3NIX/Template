using Xunit;
using Template.Commands.Identity.RoleCommands;

namespace Template.CommandHandler.Tests.Identity
{
    public class RoleCommandHandlerTests
    {
        [Fact]
        public void CreateRoleCommand_WithValidData_CreatesCommand()
        {
            var command = new CreateRoleCommand
            {
                TraceId = Guid.NewGuid(),
                RoleName = "Admin",
                Description = "Administrator role",
                IsSystem = false
            };

            Assert.NotNull(command);
            Assert.Equal("Admin", command.RoleName);
            Assert.NotEqual(Guid.Empty, command.TraceId);
        }

        [Fact]
        public void UpdateRoleCommand_WithValidData_CreatesCommand()
        {
            var roleId = Guid.NewGuid();
            var command = new UpdateRoleCommand
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId,
                RoleName = "Manager",
                Description = "Manager role",
                IsActive = true
            };

            Assert.NotNull(command);
            Assert.Equal(roleId, command.RoleId);
            Assert.Equal("Manager", command.RoleName);
        }

        [Fact]
        public void DeleteRoleCommand_WithValidId_CreatesCommand()
        {
            var roleId = Guid.NewGuid();
            var command = new DeleteRoleCommand
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId
            };

            Assert.NotNull(command);
            Assert.Equal(roleId, command.RoleId);
        }

        [Fact]
        public void AssignRoleToUserCommand_WithValidData_CreatesCommand()
        {
            var roleId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new AssignRoleToUserCommand
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId,
                UserId = userId
            };

            Assert.NotNull(command);
            Assert.Equal(roleId, command.RoleId);
            Assert.Equal(userId, command.UserId);
        }
    }
}
