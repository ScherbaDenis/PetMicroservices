using Comment.Domain.DTOs;

namespace Comment.Domain.Services
{
    public interface ICommentService : IService<CommentDto, Guid>
    {
        IEnumerable<CommentDto> GetByTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    }
}
