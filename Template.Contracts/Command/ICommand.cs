using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Contracts.Command
{
    public interface ICommand<TResult>
    {
        Guid TraceId { get; set; }
    }
}
