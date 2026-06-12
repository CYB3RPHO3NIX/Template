using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Command;

namespace Template.Commands.Identity.UserCommands
{
    public class UpdateUserCommand : ICommand<Guid>
    {
        public Guid TraceId { get; set; }
    }
}
