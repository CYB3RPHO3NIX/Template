using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Template.Commands;

namespace Template.CommandHandlers
{
    public interface ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
    {
        Task<TResult> Handle(TCommand command);
    }
}
