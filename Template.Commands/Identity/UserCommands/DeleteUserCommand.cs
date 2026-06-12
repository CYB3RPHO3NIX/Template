using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Command;

namespace Template.Commands.Identity.UserCommands
{
    public class DeleteUserCommand : ICommand<Guid>
    {
        public Guid TraceId { get; set; }
    }
}
