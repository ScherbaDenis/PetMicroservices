using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Template.Domain.DTOs;

namespace Template.Domain.Services
{
    public interface IQuestionService
    {
        Task CreateAsync(QuestionDto item, CancellationToken cancellationToken = default);
        Task UpdateAsync(QuestionDto item, CancellationToken cancellationToken = default);
        Task DeleteAsync(QuestionDto item, CancellationToken cancellationToken = default);
        IEnumerable<QuestionDto> GetAllAsync(CancellationToken cancellationToken = default);
        Task<QuestionDto?> FindAsync(Guid id, CancellationToken cancellationToken = default);
        IEnumerable<QuestionDto> Find(Func<QuestionDto, bool> predicate);
    }
}
