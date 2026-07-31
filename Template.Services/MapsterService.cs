using Microsoft.Extensions.DependencyInjection;
using Mapster;
using MapsterMapper;
using Template.Database.Domain.Entities;
using Template.Shared.Models.DTOs.Identity;

namespace Template.Services
{
    public static class MapsterService
    {
        public static IServiceCollection RegisterMapster(this IServiceCollection services)
        {
            var config = TypeAdapterConfig.GlobalSettings;

            config.NewConfig<User, UserDTO>()
                .Map(dest => dest.Id, src => src.UserId)
                .Map(dest => dest.UserName, src => src.Username);

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}
