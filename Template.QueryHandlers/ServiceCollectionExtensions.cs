using Microsoft.Extensions.DependencyInjection;
using Template.Contracts.QueryHandler;
using Template.Queries.Identity.UserQueries;
using Template.QueryHandlers.Identity.UserQueryHandlers;
using Template.Shared.Models.DTOs.Identity;

namespace Template.QueryHandlers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterQueryHandlers(this IServiceCollection services)
        {
            services.AddScoped<IQueryHandler<DoesUserExistQuery, bool>, DoesUserExistQueryHandler>();
            services.AddScoped<IQueryHandler<GetUserByIdQuery, UserDTO?>, GetUserByIdQueryHandler>();
            return services;
        }
    }
}
