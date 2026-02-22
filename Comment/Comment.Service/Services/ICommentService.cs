using Comment.Domain.DTOs;

namespace Comment.Service.Services
{
    /// <summary>
    /// Service contract for comment operations using DTOs.
    /// </summary>
    public interface ICommentService : IService<CommentDto, Guid>
    {
        /// <summary>
        /// Retrieves all comments for a specific template asynchronously.
        /// </summary>
        /// <param name="templateId">The template identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of comment DTOs.</returns>
        Task<IEnumerable<CommentDto>> GetByTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    }
}
