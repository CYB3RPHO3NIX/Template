using Microsoft.Extensions.DependencyInjection;
using Template.Contracts.ServiceBus;
using Template.Services.Jwt;
using Template.Services.Logging;

namespace Template.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBus(this IServiceCollection services)
        {
            services.AddScoped<IServiceBus, ServiceBus>();
            return services;
        }

        public static IServiceCollection AddJwtTokenService(this IServiceCollection services)
        {
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            return services;
        }

        public static IServiceCollection AddApiLogging(this IServiceCollection services)
        {
            services.AddScoped<IApiLogService, ApiLogService>();
            return services;
        }
    }
}
