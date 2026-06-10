using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Template.Database.Abstractions
{
    public interface IRepository<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        /// <summary>
        /// Returns a queryable collection of entities.
        /// </summary>
        /// <param name="asNoTracking">If true, entities are queried without change tracking for better performance. Default is true.</param>
        /// <returns>An IQueryable collection of entities.</returns>
        IQueryable<TEntity> Query(bool asNoTracking = true);

        /// <summary>
        /// Retrieves an entity by its primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        /// <param name="includes">Related entities to include in the query.</param>
        /// <returns>The entity if found, otherwise null.</returns>
        Task<TEntity?> GetByIdAsync(
            TKey id,
            params Expression<Func<TEntity,
            object>>[] includes
        );

        /// <summary>
        /// Retrieves a single entity matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The condition to filter entities.</param>
        /// <param name="asNoTracking">If true, the entity is queried without change tracking. Default is true.</param>
        /// <param name="includes">Related entities to include in the query.</param>
        /// <returns>The first entity matching the predicate, or null if none found.</returns>
        Task<TEntity?> GetSingleAsync(
            Expression<Func<TEntity, bool>> predicate,
            bool asNoTracking = true,
            params Expression<Func<TEntity,
            object>>[] includes
        );

        /// <summary>
        /// Retrieves a list of entities matching the specified criteria.
        /// </summary>
        /// <param name="predicate">Optional condition to filter entities.</param>
        /// <param name="orderBy">Optional ordering function.</param>
        /// <param name="asNoTracking">If true, entities are queried without change tracking. Default is true.</param>
        /// <param name="includes">Related entities to include in the query.</param>
        /// <returns>A list of entities matching the criteria.</returns>
        Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool asNoTracking = true,
            params Expression<Func<TEntity, object>>[] includes
        );

        /// <summary>
        /// Retrieves a paginated list of entities matching the specified criteria.
        /// </summary>
        /// <param name="predicate">Optional condition to filter entities.</param>
        /// <param name="pageIndex">Zero-based page index.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="orderBy">Optional ordering function.</param>
        /// <param name="asNoTracking">If true, entities are queried without change tracking. Default is true.</param>
        /// <param name="includes">Related entities to include in the query.</param>
        /// <returns>A paginated list of entities.</returns>
        Task<List<TEntity>> GetPagedAsync(
            Expression<Func<TEntity, bool>>? predicate,
            int pageIndex,
            int pageSize,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool asNoTracking = true,
            params Expression<Func<TEntity, object>>[] includes
        );

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddAsync(TEntity entity);

        /// <summary>
        /// Adds multiple new entities to the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Marks an entity for update.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Marks multiple entities for update.
        /// </summary>
        /// <param name="entities">The collection of entities to update.</param>
        void UpdateRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Marks an entity for deletion.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(TEntity entity);

        /// <summary>
        /// Marks multiple entities for deletion.
        /// </summary>
        /// <param name="entities">The collection of entities to remove.</param>
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}
