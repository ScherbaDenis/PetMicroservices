using WebApp.Services.DTOs;

namespace WebApp.Services
{
    public interface ICommentService
    {
        Task<CommentDto> GetByIdAsync(Guid commentId, CancellationToken cancellationToken);
        Task<CommentDto> CreateAsync(CommentDto commentDto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid commentId, CancellationToken cancellationToken);
        Task UpdateAsync(CommentDto commentDto, CancellationToken cancellationToken);
        Task<IEnumerable<CommentDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<CommentDto>> GetByTemplateIdAsync(Guid templateId, CancellationToken cancellationToken);
    }
}
