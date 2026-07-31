using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Context;
using Template.Contracts.QueryHandler;
using Template.Database.Domain.Contexts;
using Template.Queries.Identity.UserQueries;
using Template.Shared.Models.DTOs.Identity;
using Template.Shared.Models.Pagination;

namespace Template.QueryHandlers.Identity.UserQueryHandlers;

public class ListUsersQueryHandler : IQueryHandler<ListUsersQuery, PaginatedResponse<UserDTO>>
{
    private readonly TemplateDbContext _dbContext;

    public ListUsersQueryHandler(TemplateDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PaginatedResponse<UserDTO>> Handle(ListUsersQuery query)
    {
        using (LogContext.PushProperty("TraceId", query.TraceId))
        {
            Log.Information(
                "Handling ListUsersQuery - Page: {PageNumber}, PageSize: {PageSize}, Search: {SearchTerm}",
                query.PageNumber, query.PageSize, query.SearchTerm);

            var baseQuery = _dbContext.Users.AsQueryable();

            // Apply filters
            if (query.IsActive.HasValue)
            {
                baseQuery = baseQuery.Where(u => u.IsActive == query.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                baseQuery = baseQuery.Where(u =>
                    u.Username.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(searchTerm)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(searchTerm)));
            }

            // Get total count before pagination
            var totalCount = await baseQuery.CountAsync();

            // Apply sorting
            baseQuery = ApplySorting(baseQuery, query.SortBy, query.SortDescending);

            // Apply pagination
            var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
            var pageSize = query.PageSize < 1 ? 10 : (query.PageSize > 100 ? 100 : query.PageSize);
            var skip = (pageNumber - 1) * pageSize;

            var users = await baseQuery
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = users.Adapt<List<UserDTO>>();

            Log.Information(
                "ListUsersQuery result: {TotalCount} total users, page {PageNumber} returned {ItemCount} items",
                totalCount, pageNumber, userDtos.Count);

            return new PaginatedResponse<UserDTO>
            {
                Success = true,
                Items = userDtos,
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

    private static IQueryable<Template.Database.Domain.Entities.User> ApplySorting(
        IQueryable<Template.Database.Domain.Entities.User> query,
        string? sortBy,
        bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderByDescending(u => u.CreatedOn);
        }

        return sortBy.ToLower() switch
        {
            "username" => sortDescending
                ? query.OrderByDescending(u => u.Username)
                : query.OrderBy(u => u.Username),

            "email" => sortDescending
                ? query.OrderByDescending(u => u.Email)
                : query.OrderBy(u => u.Email),

            "createdon" => sortDescending
                ? query.OrderByDescending(u => u.CreatedOn)
                : query.OrderBy(u => u.CreatedOn),

            "updatedon" => sortDescending
                ? query.OrderByDescending(u => u.UpdatedOn)
                : query.OrderBy(u => u.UpdatedOn),

            _ => query.OrderByDescending(u => u.CreatedOn) // Default: newest first
        };
    }
}
