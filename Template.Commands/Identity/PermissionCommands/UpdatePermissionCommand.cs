using Template.Contracts.Command;

namespace Template.Commands.Identity.PermissionCommands
{
    public class UpdatePermissionCommand : ICommand<bool>
    {
        public Guid TraceId { get; set; }
        public Guid PermissionId { get; set; }
        public string? PermissionName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; }
    }
}
