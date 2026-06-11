using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Queries
{
    public interface IQuery<TResult>
    {
        Guid TraceId { get; set; }
    }
}
