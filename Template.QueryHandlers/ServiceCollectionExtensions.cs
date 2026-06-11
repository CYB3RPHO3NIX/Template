using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Template.Contracts.QueryHandler;
using Template.Queries.Identity.User;
using Template.QueryHandlers.Identity.User;

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
