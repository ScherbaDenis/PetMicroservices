using WebApp.Services.DTOs;

namespace WebApp.Services
{
    public interface IQuestionService
    {
        Task<QuestionDto?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
        Task<QuestionDto> CreateAsync(QuestionDto questionDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid questionId, CancellationToken cancellationToken = default);
        Task UpdateAsync(QuestionDto questionDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<QuestionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
