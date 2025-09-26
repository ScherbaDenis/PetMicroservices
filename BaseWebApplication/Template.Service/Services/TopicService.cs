using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{
    public class TopicService(ITopicRepository topicRepository) : ITopicService
    {
        private readonly ITopicRepository _topicRepository = topicRepository;

        public async Task CreateAsync(Topic item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _topicRepository.AddAsync(item, cancellationToken);
            await _topicRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Topic item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            _topicRepository.Delete(item.Id);
            await _topicRepository.SaveChangesAsync(cancellationToken);
        }

        public Task<Topic?> FindAsync(CancellationToken cancellationToken = default)
        {
            return _topicRepository.FindAsync(cancellationToken);
        }

        public IEnumerable<Topic> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _topicRepository.GetAllAsync(cancellationToken);
        }

        public async Task UpdateAsync(Topic item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);

            await _topicRepository.UpdateAsync(item, cancellationToken);
            await _topicRepository.SaveChangesAsync(cancellationToken);
        }
    }