using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Command;

namespace Template.Commands.Identity.UserCommands
{
    public class UpdateUserCommand : ICommand<bool>
    {
        public Guid TraceId { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; } = null;
        public string? Email { get; set; } = null;
        public string? Password { get; set; } = null;
        public bool? IsActive { get; set; } = null;
    }
}