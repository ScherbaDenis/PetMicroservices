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

        public async Task CreateAsync(TagDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Creating tag: {Tag}", item);

            var entity = TagMapper.ToEntity(item);
            await _tagRepository.AddAsync(entity, cancellationToken);
            await _tagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag created successfully: {Tag}", entity);
        }

        public async Task DeleteAsync(TagDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting tag: {Tag}", item);

            var entity = TagMapper.ToEntity(item);
            await _tagRepository.DeleteAsync(entity, cancellationToken);
            await _tagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag deleted successfully: {Tag}", entity);
        }

        public IEnumerable<TagDto> Find(Func<TagDto, bool> predicate)
        {
            _logger.LogInformation("Finding tag...");

            // perform find on entities then map to DTOs
            var entities = _tagRepository.Find(t => predicate(TagMapper.ToDto(t)));
            return entities.Select(TagMapper.ToDto);
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
            return TagMapper.ToDto(tag);
        }

        public IEnumerable<TagDto> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all tags...");

            var tags = _tagRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} tags", tags is ICollection<Tag> col ? col.Count : -1);

            return tags.Select(TagMapper.ToDto);
        }

        public async Task UpdateAsync(TagDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating tag: {Tag}", item);

            var entity = TagMapper.ToEntity(item);
            await _tagRepository.UpdateAsync(entity, cancellationToken);
            await _tagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag updated successfully: {Tag}", entity);
        }
    }
}
