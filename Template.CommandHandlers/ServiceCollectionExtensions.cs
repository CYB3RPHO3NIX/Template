using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Template.CommandHandlers.Identity.User;
using Template.Commands.Identity.User;
using Template.Contracts.CommandHandler;

namespace Template.CommandHandlers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<CreateUserCommand, Guid?>, CreateUserCommandHandler>();
            return services;
        }
    }
}
