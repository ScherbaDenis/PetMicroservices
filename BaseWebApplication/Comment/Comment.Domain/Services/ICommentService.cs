using Comment.Domain.DTOs;

namespace Comment.Domain.Services
{
    public interface ICommentService : IService<CommentDto, Guid>
    {
        
    }
}
