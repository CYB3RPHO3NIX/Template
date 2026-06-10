using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Template.Database.Abstractions;
using Template.Database.Repository;

namespace Template.Database.UnitOfWork
{
    /// <summary>
    /// Implementation of the Unit of Work pattern for managing database transactions.
    /// Provides repository caching and transaction management.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type to manage.</typeparam>
    public class UnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        /// <summary>
        /// Initializes a new instance of the UnitOfWork class.
        /// </summary>
        /// <param name="context">The DbContext instance to manage.</param>
        public UnitOfWork(TContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public IRepository<TEntity, TKey> Repository<TEntity, TKey>()
            where TEntity : class, IEntity<TKey>
        {
            var type = typeof(TEntity);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<TEntity, TKey>(_context);
            }

            return (IRepository<TEntity, TKey>)_repositories[type];
        }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        /// <inheritdoc/>
        public void Dispose()
            => _context.Dispose();
    }
}
