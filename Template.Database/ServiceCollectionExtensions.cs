using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Database
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the DbContext and Unit of Work pattern with dependency injection.
        /// This method automatically configures the IUnitOfWork interface with a scoped lifetime.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type to register.</typeparam>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="options">The action to configure DbContext options (e.g., connection string, provider).</param>
        /// <returns>The same service collection for method chaining.</returns>
        /// <example>
        /// <code><![CDATA[
        /// services.AddGenericDataAccess<MyDbContext>(options =>
        ///     options.UseSqlServer(connectionString));
        /// ]]></code>
        /// </example>
        public static IServiceCollection AddDatabaseAccess<TContext>(
            this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
            where TContext : DbContext
        {
            services.AddDbContext<TContext>(options);
            return services;
        }
    }
}
