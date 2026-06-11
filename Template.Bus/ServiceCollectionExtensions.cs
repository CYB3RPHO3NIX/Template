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

            RegisterCommandHandlers(services);
            RegisterQueryHandlers(services);
            RegisterEventHandlers(services);

            return services;
        }
        private static void RegisterCommandHandlers(IServiceCollection services)
        {
            //services.AddScoped<ICommandHandler<CreateUserCommand, Guid>, CreateUserCommandHandler>();
        }
        private static void RegisterQueryHandlers(IServiceCollection services)
        {
            //services.AddScoped<IQueryHandler<GetUserQuery, UserDto>, GetUserQueryHandler>();
        }
        private static void RegisterEventHandlers(IServiceCollection services)
        {
            //services.AddScoped<IEventHandler<UserCreatedEvent>, SendWelcomeEmailHandler>();
            //services.AddScoped<IEventHandler<UserCreatedEvent>, AuditHandler>();
        }
    }
}
