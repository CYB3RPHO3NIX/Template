using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.Query;
using Template.Shared.Models.Common;
using Template.Shared.Models.DTOs.Identity;

namespace Template.Queries.Identity.UserQueries
{
    public class GetUserByIdQuery : IQuery<UserDTO?>
    {
        public Guid TraceId { get; set; }
        public Guid UserId { get; set; }
    }
}
