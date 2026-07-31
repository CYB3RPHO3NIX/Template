using Template.Contracts.Query;
using Template.Shared.Models.DTOs.Identity;
using Template.Shared.Models.Pagination;

namespace Template.Queries.Identity.RoleQueries
{
    public class ListRolesQuery : IQuery<PaginatedResponse<RoleDTO>>
    {
        public Guid TraceId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "CreatedOn";
        public bool SortDescending { get; set; } = true;
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}
