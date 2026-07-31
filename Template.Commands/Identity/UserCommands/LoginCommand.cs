using Template.Contracts.Command;
using Template.Shared.Models.DTOs.Identity;

namespace Template.Commands.Identity.UserCommands;

public class LoginCommand : ICommand<LoginResponse?>
{
    public Guid TraceId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
