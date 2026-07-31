using Template.Contracts.Command;

namespace Template.Commands.Identity.PermissionCommands
{
    public class DeletePermissionCommand : ICommand<bool>
    {
        public Guid TraceId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
