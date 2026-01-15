using Microsoft.EntityFrameworkCore;
using Comment.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Comment.DataAccess.MsSql.Repositories
{
    public abstract class RepositoryBase<TEntity, ID> : IRepository<TEntity, ID>
        where TEntity : Entity<ID>
    {
        protected readonly DbContext _context;
        protected readonly ILogger logger;
        protected readonly DbSet<TEntity> _dbSet;

        protected RepositoryBase(DbContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task AddAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            await _dbSet.AddAsync(item, cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _dbSet.Remove(item);
            await Task.CompletedTask;
        }

        public virtual async Task<TEntity?> FindAsync(ID id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual IEnumerable<TEntity> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _dbSet.ToList();
        }

        public virtual async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            _dbSet.Update(item);
            await Task.CompletedTask;
        }

        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}