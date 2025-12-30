using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Template.Domain.Model;

namespace Template.Domain.Repository
{
    public interface IQuestionRepository
    {
        Task AddAsync(Question entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Question entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(Question entity, CancellationToken cancellationToken = default);
        IEnumerable<Question> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Question?> FindAsync(Guid id, CancellationToken cancellationToken = default);
        IEnumerable<Question> Find(Func<Question, bool> predicate);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
