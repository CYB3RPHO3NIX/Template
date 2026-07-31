using Template.Contracts.Command;

namespace Template.Commands.Identity.PermissionCommands
{
    public class CreatePermissionCommand : ICommand<Guid?>
    {
        public Guid TraceId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public Guid? ParentPermissionId { get; set; }
    }
}
