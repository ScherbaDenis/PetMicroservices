using Template.Domain.Model;

namespace Template.Domain.Repository
{
    public interface IRepository<Item, ID> where Item : Entity<ID>
    {
        IUnitOfWork UnitOfWork { get; }

        Task AddAsync(Topic item, CancellationToken cancellationToken = default);

        Task DeleteAsync(Topic item, CancellationToken cancellationToken = default);

        Task<Topic?> FindAsync(CancellationToken cancellationToken = default);

        IEnumerable<Topic> GetAllAsync(CancellationToken cancellationToken = default);

        Task UpdateAsync(Topic item, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        //Todo async add!
        IEnumerable<Item> Find(Func<Item, Boolean> predicate);

    }
}
