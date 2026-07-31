using Template.Contracts.Command;

namespace Template.Commands.Identity.RoleCommands
{
    public class AssignRoleToUserCommand : ICommand<Guid?>
    {
        public Guid TraceId { get; set; }
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
