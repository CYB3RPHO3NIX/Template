using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Contracts.QueryHandler;
using Template.Database.Domain.Contexts;
using Template.Queries.Identity.PermissionQueries;
using Template.Shared.Models.DTOs.Identity;
using Template.Shared.Models.Pagination;

namespace Template.QueryHandlers.Identity.PermissionQueryHandlers
{
    public class ListPermissionsQueryHandler : IQueryHandler<ListPermissionsQuery, PaginatedResponse<PermissionDTO>>
    {
        private readonly TemplateDbContext _dbContext;

        public ListPermissionsQueryHandler(TemplateDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedResponse<PermissionDTO>> Handle(ListPermissionsQuery query)
        {
            using (LogContext.PushProperty("TraceId", query.TraceId))
            {
                Log.Information(
                    "Handling ListPermissionsQuery - Page: {PageNumber}, PageSize: {PageSize}, Search: {SearchTerm}, Category: {Category}",
                    query.PageNumber, query.PageSize, query.SearchTerm, query.Category);

                var baseQuery = _dbContext.Permissions.AsQueryable();

                // Apply filters
                if (query.IsActive.HasValue)
                {
                    baseQuery = baseQuery.Where(p => p.IsActive == query.IsActive.Value);
                }

                if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                {
                    var searchTerm = query.SearchTerm.ToLower();
                    baseQuery = baseQuery.Where(p =>
                        p.PermissionName.ToLower().Contains(searchTerm) ||
                        p.PermissionCode.ToLower().Contains(searchTerm) ||
                        (p.Description != null && p.Description.ToLower().Contains(searchTerm)));
                }

                if (!string.IsNullOrWhiteSpace(query.Category))
                {
                    baseQuery = baseQuery.Where(p => p.Category == query.Category);
                }

                // Get total count before pagination
                var totalCount = await baseQuery.CountAsync();

                // Apply sorting
                baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortDescending);

                // Apply pagination
                var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
                var pageSize = query.PageSize < 1 ? 10 : (query.PageSize > 100 ? 100 : query.PageSize);
                var skip = (pageNumber - 1) * pageSize;

                var permissions = await baseQuery
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync();

                var permissionDtos = permissions.Adapt<List<PermissionDTO>>();

                Log.Information(
                    "ListPermissionsQuery result: {TotalCount} total permissions, page {PageNumber} returned {ItemCount} items",
                    totalCount, pageNumber, permissionDtos.Count);

                return new PaginatedResponse<PermissionDTO>
                {
                    Success = true,
                    Items = permissionDtos,
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

        private static IQueryable<Template.Database.Domain.Entities.Permission> ApplySorting(
            IQueryable<Template.Database.Domain.Entities.Permission> query,
            string? sortBy,
            bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderByDescending(p => p.CreatedOn);
            }

            return sortBy.ToLower() switch
            {
                "permissionname" => sortDescending
                    ? query.OrderByDescending(p => p.PermissionName)
                    : query.OrderBy(p => p.PermissionName),

                "permissioncode" => sortDescending
                    ? query.OrderByDescending(p => p.PermissionCode)
                    : query.OrderBy(p => p.PermissionCode),

                "category" => sortDescending
                    ? query.OrderByDescending(p => p.Category)
                    : query.OrderBy(p => p.Category),

                "createdon" => sortDescending
                    ? query.OrderByDescending(p => p.CreatedOn)
                    : query.OrderBy(p => p.CreatedOn),

                "updatedon" => sortDescending
                    ? query.OrderByDescending(p => p.UpdatedOn)
                    : query.OrderBy(p => p.UpdatedOn),

                _ => query.OrderByDescending(p => p.CreatedOn)
            };
        }
    }
}
