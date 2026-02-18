using WebApp.Services.DTOs;

namespace WebApp.Services
{
    public interface IAnswerService
    {
        Task<AnswerDto?> GetByIdAsync(Guid answerId, CancellationToken cancellationToken = default);
        Task<AnswerDto> CreateAsync(CreateAnswerDto answerDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid answerId, CancellationToken cancellationToken = default);
        Task UpdateAsync(UpdateAnswerDto answerDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<AnswerDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
