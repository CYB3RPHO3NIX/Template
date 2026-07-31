using Template.Contracts.Query;
using Template.Shared.Models.DTOs.Identity;

namespace Template.Queries.Identity.PermissionQueries
{
    public class GetPermissionByIdQuery : IQuery<PermissionDTO?>
    {
        public Guid TraceId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
