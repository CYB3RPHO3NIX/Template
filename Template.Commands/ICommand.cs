using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Commands
{
    public interface ICommand<TResult>
    {
        Guid TraceId { get; set; }
    }
}
