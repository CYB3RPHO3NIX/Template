using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Template.Commands;
using Template.Events;
using Template.Queries;

namespace Template.Bus
{
    public interface IBus
    {
        Task<TResult> Send<TResult>(ICommand<TResult> command);

        Task<TResult> Send<TResult>(IQuery<TResult> query);

        Task Publish(IEvent @event);
    }
}
