using Template.Contracts.Command;

namespace Template.Commands.Identity.PermissionCommands
{
    public class RemovePermissionFromRoleCommand : ICommand<bool>
    {
        public Guid TraceId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
