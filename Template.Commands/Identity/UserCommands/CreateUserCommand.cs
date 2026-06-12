using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Command;

namespace Template.Commands.Identity.UserCommands
{
    public class CreateUserCommand : ICommand<Guid?>
    {
        public Guid TraceId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
