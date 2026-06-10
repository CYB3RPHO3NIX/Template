using System;
using System.Collections.Generic;
using System.Text;

namespace Template.Database.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Retrieves or creates a repository for the specified entity type.
        /// Repositories are cached for the lifetime of the Unit of Work instance.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
        /// <returns>A repository instance for the specified entity type.</returns>
        IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class, IEntity<TKey>;

        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();
    }
}
