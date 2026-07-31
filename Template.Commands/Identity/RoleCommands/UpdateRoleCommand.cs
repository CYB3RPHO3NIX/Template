using Template.Contracts.Command;

namespace Template.Commands.Identity.RoleCommands
{
    public class UpdateRoleCommand : ICommand<bool>
    {
        public Guid TraceId { get; set; }
        public Guid RoleId { get; set; }
        public string? RoleName { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
