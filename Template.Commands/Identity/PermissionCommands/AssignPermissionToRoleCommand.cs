using Template.Contracts.Command;

namespace Template.Commands.Identity.PermissionCommands
{
    public class AssignPermissionToRoleCommand : ICommand<Guid?>
    {
        public Guid TraceId { get; set; }
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsAllowed { get; set; } = true;
    }
}
