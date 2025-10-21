using System.Linq;
using Template.Domain.DTOs;
using Template.Domain.Model;
using Template.Service.Mappers;
using Template.Domain.Repository;
using Template.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Template.Service.Services
{
    public class TopicService(IUnitOfWork unitOfWork, ILogger<TopicService> logger) : ITopicService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<TopicService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ITopicRepository _topicRepository = unitOfWork.TopicRepository;

    // use centralized TopicMapper

        public async Task CreateAsync(TopicDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Creating topic: {Topic}", item);

            var entity = item.ToEntity();
            await _topicRepository.AddAsync(entity, cancellationToken);
            await _topicRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Topic created successfully: {Topic}", item);
        }

        public async Task DeleteAsync(TopicDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting topic: {Topic}", item);

            var entity = item.ToEntity();
            await _topicRepository.DeleteAsync(entity, cancellationToken);
            await _topicRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Topic deleted successfully: {Topic}", entity);
        }

        public IEnumerable<TopicDto> Find(Func<TopicDto, bool> predicate)
        {
            _logger.LogInformation("Finding topic...");
            var entities = _topicRepository.Find(t => predicate(t.ToDto()));
            return entities.Select(t => t.ToDto());
        }

        public async Task<TopicDto?> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding topic...");
            var topic = await _topicRepository.FindAsync(id, cancellationToken);

            if (topic == null)
            {
                _logger.LogWarning("No topic found");
                return null;
            }

            _logger.LogInformation("Topic found: {Topic}", topic);
            return topic.ToDto();
        }

        public IEnumerable<TopicDto> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all topics...");

            var topics = _topicRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} topics", topics is ICollection<Topic> col ? col.Count : -1);

            return topics.Select(t => t.ToDto());
        }

        public async Task UpdateAsync(TopicDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating topic: {Topic}", item);

            var entity = item.ToEntity();
            await _topicRepository.UpdateAsync(entity, cancellationToken);
            await _topicRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Topic updated successfully: {Topic}", item);
        }
    }
}