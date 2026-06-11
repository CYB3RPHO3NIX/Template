using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Events
{
    public interface IEvent
    {
        Guid TraceId { get; set; }
    }
}
