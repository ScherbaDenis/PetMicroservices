using Microsoft.Extensions.Logging;
using System.Linq;
using Template.Domain.DTOs;
using Template.Domain.Model;
using Template.Service.Mappers;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{

    public class TagService(IUnitOfWork unitOfWork, ILogger<TagService> logger) : ITagService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<TagService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ITagRepository _tagRepository = unitOfWork.TagRepository;

    // use centralized TagMapper

        public async Task<TagDto> CreateAsync(TagDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Creating tag: {Tag}", item);

            var entity = item.ToEntity();
            await _tagRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag created successfully: {Tag}", entity);
            
            return entity.ToDto();
        }

        public async Task DeleteAsync(TagDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting tag: {@Tag}", item);

            var entity = await _tagRepository.FindAsync(item.Id, cancellationToken);
            if (entity == null)
            {
                throw new InvalidOperationException($"Tag with ID {item.Id} not found.");
            }

            await _tagRepository.DeleteAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag deleted successfully: {@Tag}", entity);
        }

        public async Task HardDeleteAsync(TagDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Hard deleting tag (admin): {@Tag}", item);

            var entity = await _tagRepository.FindAsync(item.Id, cancellationToken);
            if (entity == null)
            {
                throw new InvalidOperationException($"Tag with ID {item.Id} not found.");
            }

            await _tagRepository.HardDeleteAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag permanently deleted: {@Tag}", entity);
        }

        public async Task<IEnumerable<TagDto>> FindAsync(Func<TagDto, bool> predicate, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding tags with predicate...");

            // perform find on entities then map to DTOs
            var entities = await _tagRepository.GetAllAsync(cancellationToken);
            var dtos = entities.Select(e => e.ToDto()).Where(predicate);
            return dtos;
        }

        public async Task<TagDto?> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding tag...");

            var tag = await _tagRepository.FindAsync(id, cancellationToken);

            if (tag == null)
            {
                _logger.LogWarning("No tag found");
                return null;
            }

            _logger.LogInformation("Tag found: {Tag}", tag);
            return tag.ToDto();
        }

        public async Task<IEnumerable<TagDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all tags...");

            var tags = await _tagRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} tags", tags is ICollection<Tag> col ? col.Count : -1);

            return tags.Select(t => t.ToDto());
        }

        public async Task UpdateAsync(TagDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating tag: {@Tag}", item);

            var entity = await _tagRepository.FindAsync(item.Id, cancellationToken);
            if (entity == null)
            {
                throw new InvalidOperationException($"Tag with ID {item.Id} not found.");
            }

            // Update properties
            entity.Name = item.Name;

            await _tagRepository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag updated successfully: {@Tag}", entity);
        }
    }
}
