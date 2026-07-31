using Template.Contracts.Query;
using Template.Shared.Models.DTOs.Identity;

namespace Template.Queries.Identity.RoleQueries
{
    public class GetRoleByIdQuery : IQuery<RoleDTO?>
    {
        public Guid TraceId { get; set; }
        public Guid RoleId { get; set; }
    }
}
