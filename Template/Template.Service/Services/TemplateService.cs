using System.Linq;
using Template.Domain.DTOs;
using Template.Domain.Model;
using Template.Service.Mappers;
using Template.Domain.Repository;
using Template.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Template.Service.Services
{
    public class TemplateService(IUnitOfWork unitOfWork, ILogger<TemplateService> logger) : ITemplateService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<TemplateService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ITemplateRepository _templateRepository = unitOfWork.TemplateRepository;

        // use centralized TemplateMapper

        public async Task CreateAsync(TemplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Creating template: {Template}", item);

            var entity = item.ToEntity();
            await _templateRepository.AddAsync(entity, cancellationToken);
            await _templateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Template created successfully: {Template}", entity);
        }

        public async Task DeleteAsync(TemplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Deleting template: {Template}", item);

            var entity = item.ToEntity();
            await _templateRepository.DeleteAsync(entity, cancellationToken);
            await _templateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Template deleted successfully: {Template}", entity);
        }

        public IEnumerable<TemplateDto> Find(Func<TemplateDto, bool> predicate)
        {
            _logger.LogInformation("Finding template...");
            var entities = _templateRepository.Find(t => predicate(t.ToDto()));
            return entities.Select(t => t.ToDto());
        }

        public async Task<TemplateDto?> FindAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding template...");
            var template = await _templateRepository.FindAsync(id, cancellationToken);

            if (template == null)
            {
                _logger.LogWarning("No template found");
                return null;
            }

            _logger.LogInformation("Template found: {Template}", template);
            return template.ToDto();
        }

        public IEnumerable<TemplateDto> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all templates...");
            var templates = _templateRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} templates", templates is ICollection<Domain.Model.Template> col ? col.Count : -1);

            return templates.Select(t => t.ToDto());
        }

        public async Task UpdateAsync(TemplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Updating template: {Template}", item);

            var entity = item.ToEntity();
            await _templateRepository.UpdateAsync(entity, cancellationToken);
            await _templateRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Template updated successfully: {Template}", entity);
        }
    }
}
