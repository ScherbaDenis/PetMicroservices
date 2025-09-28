using Template.Domain.Model;

namespace Template.Domain.Repository
{
    public interface IRepository<Item, ID> where Item : Entity<ID>
    {
        IUnitOfWork UnitOfWork { get; }

        Task AddAsync(Item item, CancellationToken cancellationToken = default);

        Task DeleteAsync(Item item, CancellationToken cancellationToken = default);

        Task<Item?> FindAsync(ID id, CancellationToken cancellationToken = default);

        IEnumerable<Item> GetAllAsync(CancellationToken cancellationToken = default);

        Task UpdateAsync(Item item, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        //Todo async add!
        IEnumerable<Item> Find(Func<Item, Boolean> predicate);

    }
}
