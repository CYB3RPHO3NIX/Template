using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Template.Database.Abstractions;

namespace Template.Database.Repository
{
    /// <summary>
    /// Generic repository implementation providing standard CRUD operations for Entity Framework.
    /// </summary>
    /// <typeparam name="TEntity">The entity type that implements IEntity{TKey}.</typeparam>
    /// <typeparam name="TKey">The type of the entity's primary key.</typeparam>
    public class GenericRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// The Entity Framework DbContext instance.
        /// </summary>
        protected readonly DbContext _context;

        /// <summary>
        /// The DbSet for the entity type.
        /// </summary>
        protected readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Initializes a new instance of the GenericRepository class.
        /// </summary>
        /// <param name="context">The Entity Framework DbContext to use.</param>
        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <inheritdoc/>
        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        /// <inheritdoc/>
        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        /// <inheritdoc/>
        public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Query(asNoTracking);

            if (predicate != null)
                query = query.Where(predicate);

            query = ApplyIncludes(query, includes);

            if (orderBy != null)
                query = orderBy(query);

            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<TEntity?> GetByIdAsync(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _dbSet;
            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }

        /// <inheritdoc/>
        public async Task<List<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>>? predicate, int pageIndex, int pageSize, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Query(asNoTracking);

            if (predicate != null)
                query = query.Where(predicate);

            query = ApplyIncludes(query, includes);

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return items;
        }

        /// <inheritdoc/>
        public async Task<TEntity?> GetSingleAsync(Expression<Func<TEntity, bool>> predicate, bool asNoTracking = true, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = Query(asNoTracking);

            query = ApplyIncludes(query, includes);

            return await query.FirstOrDefaultAsync(predicate);
        }

        /// <inheritdoc/>
        public IQueryable<TEntity> Query(bool asNoTracking = true)
        {
            var query = _dbSet.AsQueryable();
            return asNoTracking ? query.AsNoTracking() : query;
        }

        /// <inheritdoc/>
        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        /// <inheritdoc/>
        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /// <inheritdoc/>
        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        /// <inheritdoc/>
        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        /// <summary>
        /// Applies eager loading for the specified navigation properties.
        /// </summary>
        /// <param name="query">The query to apply includes to.</param>
        /// <param name="includes">Navigation properties to include.</param>
        /// <returns>A query with the specified includes applied.</returns>
        private static IQueryable<TEntity> ApplyIncludes(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[] includes)
        {
            if (includes == null || includes.Length == 0)
                return query;

            return includes.Aggregate(
                query,
                (current, include) => current.Include(include));
        }
    }
}
