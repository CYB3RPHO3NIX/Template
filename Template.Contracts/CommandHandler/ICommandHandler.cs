using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Command;
using Template.Shared.Models.Common;

namespace Template.Contracts.CommandHandler
{
    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> Handle(TCommand command);
    }
}
