using Microsoft.Extensions.DependencyInjection;
using Template.CommandHandlers.Identity.UserCommandHandlers;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.CommandHandler;
using Template.Shared.Models.DTOs.Identity;

namespace Template.CommandHandlers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCommandHandlers(this IServiceCollection services)
        {
            services.AddScoped<ICommandHandler<CreateUserCommand, Guid?>, CreateUserCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateUserCommand, bool>, UpdateUserCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteUserCommand, bool>, DeleteUserCommandHandler>();
            services.AddScoped<ICommandHandler<LoginCommand, LoginResponse?>, LoginCommandHandler>();
            return services;
        }
    }
}
