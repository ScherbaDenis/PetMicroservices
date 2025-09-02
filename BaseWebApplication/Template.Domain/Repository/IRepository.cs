using Template.Domain.Model;

namespace Template.Domain.Repository
{
    public interface IRepository<Item, ID> where Item : Entity<ID>
    {
        IUnitOfWork UnitOfWork { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        //Todo async add!
        IEnumerable<Item> GetAll();

        Task<Item?> FindAsync(ID id, CancellationToken cancellationToken = default);

        public void CreateAsync(Item item, CancellationToken cancellationToken = default);

        //Todo async add!
        IEnumerable<Item> Find(Func<Item, Boolean> predicate);

        void Update(Item item);

        void Delete(ID id);

    }
}
