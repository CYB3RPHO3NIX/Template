using Microsoft.Extensions.DependencyInjection;
using Template.Contracts.QueryHandler;
using Template.Queries.Identity.PermissionQueries;
using Template.Queries.Identity.RoleQueries;
using Template.Queries.Identity.UserQueries;
using Template.QueryHandlers.Identity.PermissionQueryHandlers;
using Template.QueryHandlers.Identity.RoleQueryHandlers;
using Template.QueryHandlers.Identity.UserQueryHandlers;
using Template.Shared.Models.DTOs.Identity;
using Template.Shared.Models.Pagination;

namespace Template.QueryHandlers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterQueryHandlers(this IServiceCollection services)
        {
            // User Queries
            services.AddScoped<IQueryHandler<DoesUserExistQuery, bool>, DoesUserExistQueryHandler>();
            services.AddScoped<IQueryHandler<GetUserByIdQuery, UserDTO?>, GetUserByIdQueryHandler>();
            services.AddScoped<IQueryHandler<ListUsersQuery, PaginatedResponse<UserDTO>>, ListUsersQueryHandler>();

            // Role Queries
            services.AddScoped<IQueryHandler<GetRoleByIdQuery, RoleDTO?>, GetRoleByIdQueryHandler>();
            services.AddScoped<IQueryHandler<ListRolesQuery, PaginatedResponse<RoleDTO>>, ListRolesQueryHandler>();

            // Permission Queries
            services.AddScoped<IQueryHandler<GetPermissionByIdQuery, PermissionDTO?>, GetPermissionByIdQueryHandler>();
            services.AddScoped<IQueryHandler<ListPermissionsQuery, PaginatedResponse<PermissionDTO>>, ListPermissionsQueryHandler>();

            return services;
        }
    }
}
