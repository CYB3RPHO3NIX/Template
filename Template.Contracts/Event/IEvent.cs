using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Contracts.Event
{
    public interface IEvent
    {
        Guid TraceId { get; set; }
    }
}
