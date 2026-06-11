using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Command;
using Template.Contracts.Event;
using Template.Contracts.Query;

namespace Template.Contracts.ServiceBus
{
    public interface IServiceBus
    {
        Task<TResult> Send<TResult>(ICommand<TResult> command);

        Task<TResult> Send<TResult>(IQuery<TResult> query);

        Task Publish(IEvent @event);
    }
}
