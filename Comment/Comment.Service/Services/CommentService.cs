using Comment.Domain.DTOs;
using Comment.Domain.Repositories;

using Comment.Domain.Mappers;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

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
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment created successfully: {Comment}", entity);
        }

        public async Task DeleteAsync(CommentDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Deleting comment: {Comment}", item);

            var entity = await _commentRepository.FindAsync(item.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(entity, $"Comment with Id {item.Id} not found.");

            await _commentRepository.DeleteAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment deleted successfully: {Comment}", entity);
        }

        public async Task HardDeleteAsync(CommentDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Hard deleting comment (admin): {Comment}", item);

            var entity = await _commentRepository.FindAsync(item.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(entity, $"Comment with Id {item.Id} not found.");

            await _commentRepository.HardDeleteAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment permanently deleted: {Comment}", entity);
        }

        public async Task<IEnumerable<CommentDto>> FindAsync(
            Expression<Func<CommentDto, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding comments with predicate...");
            // Map predicate for DTO to entity if needed, or filter after mapping
            var entities = await _commentRepository.GetAllAsync(cancellationToken);
            var dtos = entities.Select(e => e.ToDto()).AsQueryable().Where(predicate);
            return dtos;
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

        public async Task<IEnumerable<CommentDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all comments...");
            var comments = await _commentRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} comments", comments is ICollection<Domain.Models.Comment> col ? col.Count : -1);

            return comments.Select(t => t.ToDto());
        }

        public async Task<IEnumerable<CommentDto>> GetByTemplateAsync(Guid templateId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving Template all comments...");
            var comments = await _commentRepository.FindAsync(
                c => c.Template.Id == templateId, cancellationToken);

            _logger.LogInformation("Retrieved Template {Count} comments", comments is ICollection<Domain.Models.Comment> col ? col.Count : -1);

            return comments.Select(t => t.ToDto());
        }

        public async Task UpdateAsync(CommentDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Updating comment: {Comment}", item);

            var dbEntity = await _commentRepository.FindAsync(item.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(dbEntity, $"Comment with Id {item.Id} not found.");  
            
            dbEntity.UpdateFromDto(item);
            await _commentRepository.UpdateAsync(dbEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Comment updated successfully: {Comment}", dbEntity);
        }

        // Optional: Pagination support
        public async Task<(IEnumerable<CommentDto> Items, int TotalCount)> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<CommentDto, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving paged comments...");
            var entitiesPaged = await _commentRepository.GetPagedAsync(
                pageIndex, pageSize, c => predicate == null || predicate.Compile().Invoke(c.ToDto()), cancellationToken);

            var dtos = entitiesPaged.Items.Select(e => e.ToDto());
            return (dtos, entitiesPaged.TotalCount);
        }

        public async Task<IEnumerable<CommentDto>> GetAllDeletedAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all deleted comments (admin)...");
            var comments = await _commentRepository.GetAllDeletedAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} deleted comments", comments is ICollection<Domain.Models.Comment> col ? col.Count : -1);

            return comments.Select(t => t.ToDto());
        }

        public async Task<CommentDto?> FindDeletedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding deleted comment (admin): {Id}", id);
            var comment = await _commentRepository.FindDeletedAsync(id, cancellationToken);

            if (comment == null)
            {
                _logger.LogWarning("No deleted comment found with Id: {Id}", id);
                return null;
            }

            _logger.LogInformation("Deleted comment found: {Comment}", comment);
            return comment.ToDto();
        }
    }
}
