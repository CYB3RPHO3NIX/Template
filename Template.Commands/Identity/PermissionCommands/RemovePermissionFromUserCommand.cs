using Template.Contracts.Command;

namespace Template.Commands.Identity.PermissionCommands
{
    public class RemovePermissionFromUserCommand : ICommand<bool>
    {
        public Guid TraceId { get; set; }
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
