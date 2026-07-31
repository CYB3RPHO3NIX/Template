using Microsoft.Extensions.DependencyInjection;
using Template.CommandHandlers.Identity.PermissionCommandHandlers;
using Template.CommandHandlers.Identity.RoleCommandHandlers;
using Template.CommandHandlers.Identity.UserCommandHandlers;
using Template.Commands.Identity.PermissionCommands;
using Template.Commands.Identity.RoleCommands;
using Template.Commands.Identity.UserCommands;
using Template.Contracts.CommandHandler;
using Template.Shared.Models.DTOs.Identity;

namespace Template.CommandHandlers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterCommandHandlers(this IServiceCollection services)
        {
            // User Commands
            services.AddScoped<ICommandHandler<CreateUserCommand, Guid?>, CreateUserCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateUserCommand, bool>, UpdateUserCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteUserCommand, bool>, DeleteUserCommandHandler>();
            services.AddScoped<ICommandHandler<LoginCommand, LoginResponse?>, LoginCommandHandler>();

            // Role Commands
            services.AddScoped<ICommandHandler<CreateRoleCommand, Guid?>, CreateRoleCommandHandler>();
            services.AddScoped<ICommandHandler<UpdateRoleCommand, bool>, UpdateRoleCommandHandler>();
            services.AddScoped<ICommandHandler<DeleteRoleCommand, bool>, DeleteRoleCommandHandler>();
            services.AddScoped<ICommandHandler<AssignRoleToUserCommand, Guid?>, AssignRoleToUserCommandHandler>();
            services.AddScoped<ICommandHandler<RemoveRoleFromUserCommand, bool>, RemoveRoleFromUserCommandHandler>();

            // Permission Commands
            services.AddScoped<ICommandHandler<CreatePermissionCommand, Guid?>, CreatePermissionCommandHandler>();
            services.AddScoped<ICommandHandler<UpdatePermissionCommand, bool>, UpdatePermissionCommandHandler>();
            services.AddScoped<ICommandHandler<DeletePermissionCommand, bool>, DeletePermissionCommandHandler>();
            services.AddScoped<ICommandHandler<AssignPermissionToRoleCommand, Guid?>, AssignPermissionToRoleCommandHandler>();
            services.AddScoped<ICommandHandler<RemovePermissionFromRoleCommand, bool>, RemovePermissionFromRoleCommandHandler>();
            services.AddScoped<ICommandHandler<AssignPermissionToUserCommand, Guid?>, AssignPermissionToUserCommandHandler>();
            services.AddScoped<ICommandHandler<RemovePermissionFromUserCommand, bool>, RemovePermissionFromUserCommandHandler>();

            return services;
        }
    }
}
