using System;
using System.Collections.Generic;
using System.Text;
using Template.Queries;

namespace Template.QueryHandlers
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query);
    }
}
