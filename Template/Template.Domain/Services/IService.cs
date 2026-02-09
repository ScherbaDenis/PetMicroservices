using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Domain.Services
{
    /// <summary>
    /// Service contract using DTOs between service and controller layers.
    /// Implementations should map DTOs to domain entities before calling repositories.
    /// </summary>
    public interface IService<TDto, TId>
    {
        Task<IEnumerable<TDto>> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TDto?> FindAsync(TId id, CancellationToken cancellationToken = default);

        Task<TDto> CreateAsync(TDto item, CancellationToken cancellationToken = default);

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

        Task<IEnumerable<TDto>> FindAsync(Func<TDto, bool> predicate, CancellationToken cancellationToken = default);
    }
}