using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Query;

namespace Template.Queries.Identity.UserQueries
{
    public class DoesUserExistQuery : IQuery<bool>
    {
        public Guid TraceId { get; set; }
        public string? Email { get; set; } = null;
        public string? UserName { get; set; } = null;
    }
}
