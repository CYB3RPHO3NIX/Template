using Template.Contracts.Command;

namespace Template.Commands.Identity.RoleCommands
{
    public class RemoveRoleFromUserCommand : ICommand<bool>
    {
        public Guid TraceId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
