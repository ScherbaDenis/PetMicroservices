using Comment.Domain.DTOs;
using Comment.Domain.Repositories;
using Comment.Domain.Services;
using Comment.Domain.Mappers;
using Microsoft.Extensions.Logging;

namespace Comment.Service.Services
{
    public class CommentService(IUnitOfWork unitOfWork, ILogger<CommentService> logger) : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<CommentService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ICommentRepository _commentRepository = unitOfWork.CommentRepository;

        public async Task CreateAsync(CommentDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Creating comment: {Comment}", item);

            var entity = item.ToEntity();
            await _commentRepository.AddAsync(entity, cancellationToken);
            await _commentRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment created successfully: {Comment}", entity);
        }

        public async Task DeleteAsync(CommentDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Deleting comment: {Comment}", item);

            var entity = item.ToEntity();
            await _commentRepository.DeleteAsync(entity, cancellationToken);
            await _commentRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment deleted successfully: {Comment}", entity);
        }

        public IEnumerable<CommentDto> Find(Func<CommentDto, bool> predicate)
        {
            _logger.LogInformation("Finding comment...");
            var entities = _commentRepository.Find(t => predicate(t.ToDto()));
            return entities.Select(t => t.ToDto());
        }

        public async Task<CommentDto?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding comment...");
            var comment = await _commentRepository.FindAsync(id, cancellationToken);

            if (comment == null)
            {
                _logger.LogWarning("No comment found");
                return null;
            }

            _logger.LogInformation("Comment found: {Comment}", comment);
            return comment.ToDto();
        }

        public IEnumerable<CommentDto> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all comments...");
            var comments = _commentRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} comments", comments is ICollection<Domain.Models.Comment> col ? col.Count : -1);

            return comments.Select(t => t.ToDto());
        }

        public IEnumerable<CommentDto> GetByTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving Template all comments...");
            var comments = _commentRepository.GetAllAsync(cancellationToken).Where(comment => comment.Template.Id == templateId);

            _logger.LogInformation("Retrieved Template {Count} comments", comments is ICollection<Domain.Models.Comment> col ? col.Count : -1);

            return comments.Select(t => t.ToDto());
        }

        public async Task UpdateAsync(CommentDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Updating comment: {Comment}", item);

            var entity = item.ToEntity();
            await _commentRepository.UpdateAsync(entity, cancellationToken);
            await _commentRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment updated successfully: {Comment}", entity);
        }
    }
}
