using Xunit;
using Template.Commands.Identity.PermissionCommands;

namespace Template.CommandHandler.Tests.Identity
{
    public class PermissionCommandHandlerTests
    {
        [Fact]
        public void CreatePermissionCommand_WithValidData_CreatesCommand()
        {
            var command = new CreatePermissionCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionName = "Create Users",
                PermissionCode = "USER_CREATE",
                Description = "Permission to create new users",
                Category = "Users"
            };

            Assert.NotNull(command);
            Assert.Equal("Create Users", command.PermissionName);
            Assert.Equal("USER_CREATE", command.PermissionCode);
        }

        [Fact]
        public void UpdatePermissionCommand_WithValidData_CreatesCommand()
        {
            var permissionId = Guid.NewGuid();
            var command = new UpdatePermissionCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionId = permissionId,
                PermissionName = "Delete Users",
                Description = "Permission to delete users",
                Category = "Users",
                IsActive = true
            };

            Assert.NotNull(command);
            Assert.Equal(permissionId, command.PermissionId);
            Assert.Equal("Delete Users", command.PermissionName);
        }

        [Fact]
        public void DeletePermissionCommand_WithValidId_CreatesCommand()
        {
            var permissionId = Guid.NewGuid();
            var command = new DeletePermissionCommand
            {
                TraceId = Guid.NewGuid(),
                PermissionId = permissionId
            };

            Assert.NotNull(command);
            Assert.Equal(permissionId, command.PermissionId);
        }

        [Fact]
        public void AssignPermissionToRoleCommand_WithValidData_CreatesCommand()
        {
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();
            var command = new AssignPermissionToRoleCommand
            {
                TraceId = Guid.NewGuid(),
                RoleId = roleId,
                PermissionId = permissionId,
                IsAllowed = true
            };

            Assert.NotNull(command);
            Assert.Equal(roleId, command.RoleId);
            Assert.Equal(permissionId, command.PermissionId);
        }
    }
}
