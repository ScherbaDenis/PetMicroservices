using System.Linq.Expressions;

namespace Comment.Domain.Services
{
    /// <summary>
    /// Service contract using DTOs between service and controller layers.
    /// Implementations should map DTOs to domain entities before calling repositories.
    /// </summary>
    /// <typeparam name="TDto">The DTO type managed by the service.</typeparam>
    /// <typeparam name="TId">The type of the DTO's identifier.</typeparam>
    public interface IService<TDto, TId>
    {
        /// <summary>
        /// Asynchronously retrieves all DTOs.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A collection of all DTOs.</returns>
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously finds a DTO by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the DTO.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>The DTO if found; otherwise, null.</returns>
        Task<TDto?> FindAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously creates a new DTO.
        /// </summary>
        /// <param name="item">The DTO to create.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task CreateAsync(TDto item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously updates an existing DTO.
        /// </summary>
        /// <param name="item">The DTO to update.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task UpdateAsync(TDto item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously deletes a DTO (soft delete - sets IsDeleted = true).
        /// </summary>
        /// <param name="item">The DTO to delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task DeleteAsync(TDto item, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously permanently deletes a DTO from the database (hard delete).
        /// This is intended for admin use only.
        /// </summary>
        /// <param name="item">The DTO to permanently delete.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task HardDeleteAsync(TDto item, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Asynchronously finds DTOs matching the specified predicate.
        /// </summary>
        /// <param name="predicate">The filter expression.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>A collection of matching DTOs.</returns>
        Task<IEnumerable<TDto>> FindAsync(
          Expression<Func<TDto, bool>> predicate,
          CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously retrieves a paged list of DTOs matching the specified predicate.
        /// </summary>
        /// <param name="pageIndex">The zero-based page index.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="predicate">An optional filter expression.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        /// <returns>
        /// A tuple containing the collection of DTOs for the page and the total count of matching DTOs.
        /// </returns>
        Task<(IEnumerable<TDto> Items, int TotalCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<TDto, bool>>? predicate = null,
            CancellationToken cancellationToken = default);
    }
}