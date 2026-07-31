using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Template.Contracts.EventHandler;

namespace Template.EventHandlers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterEventHandlers(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var eventHandlerType = typeof(IEventHandler<>);

            var eventHandlers = assembly.GetTypes()
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == eventHandlerType))
                .ToList();

            foreach (var handler in eventHandlers)
            {
                var handlerInterface = handler.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == eventHandlerType);

                services.AddScoped(handlerInterface, handler);
            }

            return services;
        }
    }
}
