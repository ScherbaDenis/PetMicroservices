using System.Linq.Expressions;
using Template.Domain.Model;

namespace Template.Domain.Repository
{
    /// <summary>
    /// Generic repository interface for CRUD and query operations on entities.
    /// </summary>
    /// <typeparam name="Item">The type of entity managed by the repository.</typeparam>
    /// <typeparam name="ID">The type of the entity's identifier.</typeparam>
    public interface IRepository<Item, ID> where Item : Entity<ID>
    {
        /// <summary>
        /// Asynchronously adds a new item to the repository.
        /// Changes will be saved to the database when <see cref="IUnitOfWork.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <param name="item">The entity to add.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task AddAsync(Item item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously deletes an entity from the repository.
        /// Changes will be saved to the database when <see cref="IUnitOfWork.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <param name="item">The entity to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task DeleteAsync(Item item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously finds an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The entity if found; otherwise, null.</returns>
        Task<Item?> FindAsync(ID id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves all entities from the repository.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A collection of all entities.</returns>
        Task<IEnumerable<Item>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously updates an existing entity in the repository.
        /// Changes will be saved to the database when <see cref="IUnitOfWork.SaveChangesAsync(CancellationToken)" /> is called.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task UpdateAsync(Item item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously finds entities matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A collection of matching entities.</returns>
        Task<IEnumerable<Item>> FindAsync(
            Expression<Func<Item, bool>> predicate,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves a paged list of entities matching the specified predicate.
        /// </summary>
        /// <param name="pageIndex">The zero-based page index.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="predicate">An optional filter expression.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A tuple containing the collection of entities for the page and the total count of matching entities.
        /// </returns>
        Task<(IEnumerable<Item> Items, int TotalCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<Item, bool>>? predicate = null,
            CancellationToken cancellationToken = default);
    }
}
