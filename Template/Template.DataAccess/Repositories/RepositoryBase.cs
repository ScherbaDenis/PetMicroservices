using Microsoft.EntityFrameworkCore;
using Template.Domain.Repository;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Template.DataAccess.MsSql.Repositories
{
    /// <summary>
    /// Provides a base implementation of the generic repository pattern for CRUD and query operations.
    /// </summary>
    /// <typeparam name="TEntity">The entity type managed by the repository.</typeparam>
    /// <typeparam name="ID">The type of the entity's identifier.</typeparam>
    public abstract class RepositoryBase<TEntity, ID> : IRepository<TEntity, ID>
        where TEntity : Entity<ID>
    {
        protected readonly DbContext _context;
        protected readonly ILogger logger;
        protected readonly DbSet<TEntity> _dbSet;

        /// <summary>
        /// Initializes a new instance of the RepositoryBase class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger instance.</param>
        protected RepositoryBase(DbContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger;
            _dbSet = _context.Set<TEntity>();
        }

        /// <inheritdoc/>
        public virtual async Task AddAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.DateCreated = DateTime.UtcNow;
            item.DateUpdated = DateTime.UtcNow;
            item.IsDeleted = false;
            await _dbSet.AddAsync(item, cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task DeleteAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.IsDeleted = true;
            item.DateUpdated = DateTime.UtcNow;
            _dbSet.Update(item);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task HardDeleteAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _dbSet.Remove(item);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity?> FindAsync(ID id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (entity != null && entity.IsDeleted)
            {
                return null;
            }
            return entity;
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            item.DateUpdated = DateTime.UtcNow;
            _dbSet.Update(item);
            await Task.CompletedTask;
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> FindAsync(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = _dbSet.Where(e => !e.IsDeleted);
            if (predicate != null)
                query = query.Where(predicate);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        /// <inheritdoc/>
        public virtual async Task<IEnumerable<TEntity>> GetAllDeletedAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(e => e.IsDeleted).ToListAsync(cancellationToken);
        }

        /// <inheritdoc/>
        public virtual async Task<TEntity?> FindDeletedAsync(ID id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (entity != null && entity.IsDeleted)
            {
                return entity;
            }
            return null;
        }
    }
}
