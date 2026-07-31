using System;
using Template.Contracts.Command;
using Template.Contracts.Query;
using Template.Contracts.Event;

namespace Template.Bus
{
    public interface IBus
    {
        Task<TResult> Send<TResult>(ICommand<TResult> command);

        Task<TResult> Send<TResult>(IQuery<TResult> query);

        Task Publish(IEvent domainEvent);
    }
}
