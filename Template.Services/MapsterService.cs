using Microsoft.Extensions.DependencyInjection;
using System;
using Mapster;
using System.Collections.Generic;
using System.Text;
using MapsterMapper;

namespace Template.Services
{
    public static class MapsterService
    {
        public static IServiceCollection RegisterMapster(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}
