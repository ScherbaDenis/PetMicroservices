using Microsoft.Extensions.Logging;
using Template.Domain.Model;
using Template.Domain.Repository;
using Template.Domain.Services;

namespace Template.Service.Services
{
    public class TamplateService(ITamplateRepository tamplateRepository, ILogger<TamplateService> logger) : ITamplateService
    {
        private readonly ITamplateRepository _tamplateRepository = tamplateRepository ?? throw new ArgumentNullException(nameof(tamplateRepository));
        private readonly ILogger<TamplateService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task CreateAsync(Tamplate item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _logger.LogInformation("Creating tamplate: {Tamplate}", item);

            await _tamplateRepository.AddAsync(item, cancellationToken);
            await _tamplateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tamplate created successfully: {Tamplate}", item);
        }

        public async Task DeleteAsync(Tamplate item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _logger.LogInformation("Deleting tamplate: {Tamplate}", item);

            await _tamplateRepository.DeleteAsync(item, cancellationToken);
            await _tamplateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tamplate deleted successfully: {Tamplate}", item);
        }

        public IEnumerable<Tamplate> Find(Func<Tamplate, bool> predicate)
        {
            _logger.LogInformation("Finding tamplate...");

            return _tamplateRepository.Find(predicate);
        }

        public async Task<Tamplate?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding tamplate...");

            var tamplate = await _tamplateRepository.FindAsync(id, cancellationToken);

            if (tamplate == null)
                _logger.LogWarning("No tamplate found");
            else
                _logger.LogInformation("Tamplate found: {Tamplate}", tamplate);

            return tamplate;
        }

        public IEnumerable<Tamplate> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all tamplates...");

            var tamplates = _tamplateRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} tamplates", tamplates is ICollection<Tamplate> col ? col.Count : -1);

            return tamplates;
        }

        public async Task UpdateAsync(Tamplate item, CancellationToken cancellationToken = default)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            _logger.LogInformation("Updating tamplate: {Tamplate}", item);

            await _tamplateRepository.UpdateAsync(item, cancellationToken);
            await _tamplateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tamplate updated successfully: {Tamplate}", item);
        }
    }
}
