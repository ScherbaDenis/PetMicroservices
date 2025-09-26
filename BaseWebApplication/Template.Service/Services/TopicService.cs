using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Template.Service.Services
{
    public class TopicService(ITopicRepository topicRepository, ILogger<TopicService> logger) : ITopicService
    {
        private readonly ITopicRepository _topicRepository = topicRepository ?? throw new ArgumentNullException(nameof(topicRepository));
        private readonly ILogger<TopicService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task CreateAsync(Topic item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Creating topic: {Topic}", item);

            await _topicRepository.AddAsync(item, cancellationToken);
            await _topicRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Topic created successfully: {Topic}", item);
        }

        public async Task DeleteAsync(Topic item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Deleting topic: {Topic}", item);

            await _topicRepository.DeleteAsync(item, cancellationToken);
            await _topicRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Topic deleted successfully: {Topic}", item);
        }

        public IEnumerable<Topic> Find(Func<Topic, bool> predicate)
        {
            _logger.LogInformation("Finding topic...");

            return _topicRepository.Find(predicate);   
        }

        public async Task<Topic?> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding topic...");

            var topic = await _topicRepository.FindAsync(id, cancellationToken);

            if (topic == null)
                _logger.LogWarning("No topic found");
            else
                _logger.LogInformation("Topic found: {Topic}", topic);

            return topic;
        }

        public IEnumerable<Topic> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all topics...");

            var topics = _topicRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} topics", topics is ICollection<Topic> col ? col.Count : -1);

            return topics;
        }

        public async Task UpdateAsync(Topic item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _logger.LogInformation("Updating topic: {Topic}", item);

            await _topicRepository.UpdateAsync(item, cancellationToken);
            await _topicRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Topic updated successfully: {Topic}", item);
        }
    }
}