using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Contracts.Query
{
    public interface IQuery<TResult>
    {
        Guid TraceId { get; set; }
    }
}
