using Comment.Domain.DTOs;

namespace Comment.Domain.Services
{
    public interface ICommentService : IService<CommentDto, Guid>
    {
        IEnumerable<CommentDto> GetByTamplateAsync(Guid tamplateId, CancellationToken cancellationToken = default);
    }
}
