using Template.Domain.Repository;

namespace Template.Domain.Services
{
    public interface IService<Item, ID> where Item : Entity<ID>
    {
        IEnumerable<Item> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Item?> FindAsync(ID id, CancellationToken cancellationToken = default);

        Task CreateAsync(Item item, CancellationToken cancellationToken = default);

        Task UpdateAsync(Item item, CancellationToken cancellationToken = default);

        Task DeleteAsync(Item item, CancellationToken cancellationToken = default);

        IEnumerable<Item> Find(Func<Item, Boolean> predicate);
    }
}