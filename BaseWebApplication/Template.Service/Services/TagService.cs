using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{

    public class TagService(ITagRepository tagRepository, ILogger<TagService> logger) : ITagService
    {
        private readonly ITagRepository _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
        private readonly ILogger<TagService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task CreateAsync(Tag item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Creating tag: {Tag}", item);

            await _tagRepository.AddAsync(item, cancellationToken);
            await _tagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag created successfully: {Tag}", item);
        }

        public async Task DeleteAsync(Tag item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting tag: {Tag}", item);

            await _tagRepository.DeleteAsync(item, cancellationToken);
            await _tagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag deleted successfully: {Tag}", item);
        }

        public IEnumerable<Tag> Find(Func<Tag, bool> predicate)
        {
            _logger.LogInformation("Finding tag...");

            return _tagRepository.Find(predicate);
        }

        public async Task<Tag?> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding tag...");

            var tag = await _tagRepository.FindAsync(id, cancellationToken);

            if (tag == null)
                _logger.LogWarning("No tag found");
            else
                _logger.LogInformation("Tag found: {Tag}", tag);

            return tag;
        }

        public IEnumerable<Tag> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all tags...");

            var tags = _tagRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} tags", tags is ICollection<Tag> col ? col.Count : -1);

            return tags;
        }

        public async Task UpdateAsync(Tag item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating tag: {Tag}", item);

            await _tagRepository.UpdateAsync(item, cancellationToken);
            await _tagRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tag updated successfully: {Tag}", item);
        }
    }
}
