using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.QueryHandler;
using Template.Queries.Identity.UserQueries;
using Template.QueryHandlers.Identity.UserQueryHandlers;

namespace Template.QueryHandlers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterQueryHandlers(this IServiceCollection services)
        {
            services.AddScoped<IQueryHandler<DoesUserExistQuery, bool>, DoesUserExistQueryHandler>();
            return services;
        }
    }
}
