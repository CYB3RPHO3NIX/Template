using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Query;
using Template.Shared.Models.Common;

namespace Template.Contracts.QueryHandler
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        Task<TResult> Handle(TQuery query);
    }
}
