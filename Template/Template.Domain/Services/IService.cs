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
        IEnumerable<TDto> GetAllAsync(CancellationToken cancellationToken = default);

        Task<TDto?> FindAsync(TId id, CancellationToken cancellationToken = default);

        Task CreateAsync(TDto item, CancellationToken cancellationToken = default);

        Task UpdateAsync(TDto item, CancellationToken cancellationToken = default);

        Task DeleteAsync(TDto item, CancellationToken cancellationToken = default);

        IEnumerable<TDto> Find(Func<TDto, bool> predicate);
    }
}