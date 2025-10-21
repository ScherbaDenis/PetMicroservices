using System.Linq;
using Template.Domain.DTOs;
using Template.Domain.Model;
using Template.Service.Mappers;
using Template.Domain.Repository;
using Template.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Template.Service.Services
{
    public class TamplateService(IUnitOfWork unitOfWork, ILogger<TamplateService> logger) : ITamplateService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<TamplateService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ITamplateRepository _tamplateRepository = unitOfWork.TamplateRepository;

        // use centralized TamplateMapper

        public async Task CreateAsync(TamplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Creating tamplate: {Tamplate}", item);

            var entity = TamplateMapper.ToEntity(item);
            await _tamplateRepository.AddAsync(entity, cancellationToken);
            await _tamplateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tamplate created successfully: {Tamplate}", entity);
        }

        public async Task DeleteAsync(TamplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Deleting tamplate: {Tamplate}", item);

            var entity = TamplateMapper.ToEntity(item);
            await _tamplateRepository.DeleteAsync(entity, cancellationToken);
            await _tamplateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tamplate deleted successfully: {Tamplate}", entity);
        }

        public IEnumerable<TamplateDto> Find(Func<TamplateDto, bool> predicate)
        {
            _logger.LogInformation("Finding tamplate...");
            var entities = _tamplateRepository.Find(t => predicate(TamplateMapper.ToDto(t)));
            return entities.Select(TamplateMapper.ToDto);
        }

        public async Task<TamplateDto?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding tamplate...");
            var tamplate = await _tamplateRepository.FindAsync(id, cancellationToken);

            if (tamplate == null)
            {
                _logger.LogWarning("No tamplate found");
                return null;
            }

            _logger.LogInformation("Tamplate found: {Tamplate}", tamplate);
            return TamplateMapper.ToDto(tamplate);
        }

        public IEnumerable<TamplateDto> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all tamplates...");
            var tamplates = _tamplateRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} tamplates", tamplates is ICollection<Tamplate> col ? col.Count : -1);

            return tamplates.Select(TamplateMapper.ToDto);
        }

        public async Task UpdateAsync(TamplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Updating tamplate: {Tamplate}", item);

            var entity = TamplateMapper.ToEntity(item);
            await _tamplateRepository.UpdateAsync(entity, cancellationToken);
            await _tamplateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Tamplate updated successfully: {Tamplate}", entity);
        }
    }
}
