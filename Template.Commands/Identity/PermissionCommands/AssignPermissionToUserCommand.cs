using Template.Contracts.Command;

namespace Template.Commands.Identity.PermissionCommands
{
    public class AssignPermissionToUserCommand : ICommand<Guid?>
    {
        public Guid TraceId { get; set; }
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
        public bool IsAllowed { get; set; } = true;
        public string? Reason { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
