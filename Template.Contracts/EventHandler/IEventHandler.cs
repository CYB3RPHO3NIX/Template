using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Event;

namespace Template.Contracts.EventHandler
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task Handle(TEvent @event);
    }
}
