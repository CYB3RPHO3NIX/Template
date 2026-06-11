using System;
using System.Collections.Generic;
using System.Text;
using Template.Events;

namespace Template.EventHandlers
{
    public interface IEventHandler<TEvent> where TEvent : IEvent
    {
        Task Handle(TEvent @event);
    }
}
