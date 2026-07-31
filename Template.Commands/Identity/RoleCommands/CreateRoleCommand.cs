using Template.Contracts.Command;

namespace Template.Commands.Identity.RoleCommands
{
    public class CreateRoleCommand : ICommand<Guid?>
    {
        public Guid TraceId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsSystem { get; set; }
    }
}
