namespace Template.Domain.Services
{
    public interface IService<Item> 
    {
        IEnumerable<Item> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Item?> FindAsync(CancellationToken cancellationToken = default);

        public void CreateAsync(Item item, CancellationToken cancellationToken = default);

        void UpdateAsync(Item item, CancellationToken cancellationToken = default);

        void DeleteAsync(Item item, CancellationToken cancellationToken = default);
    }
}