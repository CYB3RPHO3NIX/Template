using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Template.CommandHandlers;
using Template.EventHandlers;
using Template.QueryHandlers;

namespace Template.Bus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBus(this IServiceCollection services)
        {
            services.AddScoped<IBus, Bus>();
            return services;
        }
    }
}
