namespace Template.Domain.Services
{
    public interface IService<Item>
    {
        IEnumerable<Item> GetAllAsync(CancellationToken cancellationToken = default);

        Task<Item?> FindAsync(CancellationToken cancellationToken = default);

        Task CreateAsync(Item item, CancellationToken cancellationToken = default);

        Task UpdateAsync(Item item, CancellationToken cancellationToken = default);

        Task DeleteAsync(Item item, CancellationToken cancellationToken = default);
    }
}