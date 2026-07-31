using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Contracts.QueryHandler;
using Template.Database.Domain.Contexts;
using Template.Queries.Identity.RoleQueries;
using Template.Shared.Models.DTOs.Identity;
using Template.Shared.Models.Pagination;

namespace Template.QueryHandlers.Identity.RoleQueryHandlers
{
    public class ListRolesQueryHandler : IQueryHandler<ListRolesQuery, PaginatedResponse<RoleDTO>>
    {
        private readonly TemplateDbContext _dbContext;

        public ListRolesQueryHandler(TemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedResponse<RoleDTO>> Handle(ListRolesQuery query)
        {
            using (LogContext.PushProperty("TraceId", query.TraceId))
            {
                Log.Information(
                    "Handling ListRolesQuery - Page: {PageNumber}, PageSize: {PageSize}, Search: {SearchTerm}",
                    query.PageNumber, query.PageSize, query.SearchTerm);

                var baseQuery = _dbContext.Roles.AsQueryable();

                // Apply filters
                if (query.IsActive.HasValue)
                {
                    baseQuery = baseQuery.Where(r => r.IsActive == query.IsActive.Value);
                }

                if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                {
                    var searchTerm = query.SearchTerm.ToLower();
                    baseQuery = baseQuery.Where(r =>
                        r.RoleName.ToLower().Contains(searchTerm) ||
                        (r.Description != null && r.Description.ToLower().Contains(searchTerm)));
                }

                // Get total count before pagination
                var totalCount = await baseQuery.CountAsync();

                // Apply sorting
                baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortDescending);

                // Apply pagination
                var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
                var pageSize = query.PageSize < 1 ? 10 : (query.PageSize > 100 ? 100 : query.PageSize);
                var skip = (pageNumber - 1) * pageSize;

                var roles = await baseQuery
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                var roleDtos = roles.Adapt<List<RoleDTO>>();

                Log.Information(
                    "ListRolesQuery result: {TotalCount} total roles, page {PageNumber} returned {ItemCount} items",
                    totalCount, pageNumber, roleDtos.Count);

                return new PaginatedResponse<RoleDTO>
                {
                    Success = true,
                    Items = roleDtos,
                    Pagination = new PaginationMetadata
                    {
                        PageNumber = pageNumber,
                        PageSize = pageSize,
                        TotalCount = totalCount
                    },
                    TraceId = query.TraceId.ToString(),
                    Timestamp = DateTime.UtcNow
                };
            }
        }

        private static IQueryable<Template.Database.Domain.Entities.Role> ApplySorting(
            IQueryable<Template.Database.Domain.Entities.Role> query,
            string? sortBy,
            bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderByDescending(r => r.CreatedOn);
            }

            return sortBy.ToLower() switch
            {
                "rolename" => sortDescending
                    ? query.OrderByDescending(r => r.RoleName)
                    : query.OrderBy(r => r.RoleName),

                "createdon" => sortDescending
                    ? query.OrderByDescending(r => r.CreatedOn)
                    : query.OrderBy(r => r.CreatedOn),

                "updatedon" => sortDescending
                    ? query.OrderByDescending(r => r.UpdatedOn)
                    : query.OrderBy(r => r.UpdatedOn),

                _ => query.OrderByDescending(r => r.CreatedOn)
            };
        }
    }
}
