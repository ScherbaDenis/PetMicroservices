using Comment.Domain.Repositories;
using System.Linq.Expressions;

namespace Comment.Domain.Repositories
{
    public interface IRepository<Item, ID> where Item : Entity<ID>
    {
        Task AddAsync(Item item, CancellationToken cancellationToken = default);

        Task DeleteAsync(Item item, CancellationToken cancellationToken = default);

        Task<Item?> FindAsync(ID id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Item>> GetAllAsync(CancellationToken cancellationToken = default);

        Task UpdateAsync(Item item, CancellationToken cancellationToken = default);

        Task<IEnumerable<Item>> FindAsync(
            Expression<Func<Item, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<(IEnumerable<Item> Items, int TotalCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<Item, bool>>? predicate = null,
            CancellationToken cancellationToken = default);
    }
}
