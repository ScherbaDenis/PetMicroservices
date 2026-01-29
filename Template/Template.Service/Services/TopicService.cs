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
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Topic created successfully: {Topic}", item);
        }

        public async Task DeleteAsync(TopicDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting topic: {Topic}", item);

            var entity = item.ToEntity();
            await _topicRepository.DeleteAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Topic deleted successfully: {Topic}", entity);
        }

        public async Task<IEnumerable<TopicDto>> FindAsync(Func<TopicDto, bool> predicate, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding topics with predicate...");
            var entities = await _topicRepository.GetAllAsync(cancellationToken);
            var dtos = entities.Select(e => e.ToDto()).Where(predicate);
            return dtos;
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

        public async Task<IEnumerable<TopicDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all topics...");

            var topics = await _topicRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} topics", topics is ICollection<Topic> col ? col.Count : -1);

            return topics.Select(t => t.ToDto());
        }

        public async Task UpdateAsync(TopicDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating topic: {Topic}", item);

            var entity = item.ToEntity();
            await _topicRepository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Topic updated successfully: {Topic}", item);
        }
    }
}