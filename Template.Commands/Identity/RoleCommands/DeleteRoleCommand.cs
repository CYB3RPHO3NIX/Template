using Template.Contracts.Command;

namespace Template.Commands.Identity.RoleCommands
{
    public class DeleteRoleCommand : ICommand<bool>
    {
        public Guid TraceId { get; set; }
        public Guid RoleId { get; set; }
    }
}
