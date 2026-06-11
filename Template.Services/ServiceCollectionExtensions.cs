using Microsoft.Extensions.DependencyInjection;
using Template.Contracts.ServiceBus;

namespace Template.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBus(this IServiceCollection services)
        {
            services.AddScoped<IServiceBus, ServiceBus>();
            return services;
        }
    }
}
