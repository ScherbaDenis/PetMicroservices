using System.Linq;
using Template.Domain.DTOs;
using Template.Domain.Model;
using Template.Service.Mappers;
using Template.Domain.Repository;
using Template.Service.Services;
using Microsoft.Extensions.Logging;
using MassTransit;
using Shared.Messaging.Events;

namespace Template.Service.Services
{
    public class TemplateService(IUnitOfWork unitOfWork, ILogger<TemplateService> logger, IPublishEndpoint publishEndpoint) : ITemplateService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<TemplateService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
        private readonly ITemplateRepository _templateRepository = unitOfWork.TemplateRepository;

        // use centralized TemplateMapper

        public async Task<TemplateDto> CreateAsync(TemplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Creating template: {Template}", item);

            var entity = item.ToEntity();
            await _templateRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Template created successfully: {Template}", entity);

            await _publishEndpoint.Publish(new TemplateCreatedEvent
            {
                Id = entity.Id,
                Title = entity.Title
            }, cancellationToken);

            _logger.LogInformation("Published TemplateCreatedEvent for template: {TemplateId}", entity.Id);
            
            return entity.ToDto();
        }

        public async Task DeleteAsync(TemplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Deleting template: {Template}", item);

            var entity = await _templateRepository.FindAsync(item.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(entity, $"Template with Id {item.Id} not found.");

            await _templateRepository.DeleteAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Template deleted successfully: {Template}", entity);
        }

        public async Task HardDeleteAsync(TemplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Hard deleting template (admin): {Template}", item);

            var entity = await _templateRepository.FindAsync(item.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(entity, $"Template with Id {item.Id} not found.");

            await _templateRepository.HardDeleteAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Template permanently deleted: {Template}", entity);
        }

        public async Task<IEnumerable<TemplateDto>> FindAsync(
            Func<TemplateDto, bool> predicate,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding templates with predicate...");
            var entities = await _templateRepository.GetAllAsync(cancellationToken);
            var dtos = entities.Select(e => e.ToDto()).Where(predicate);
            return dtos;
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

        public async Task<IEnumerable<TemplateDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all templates...");
            var templates = await _templateRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} templates", templates is ICollection<Domain.Model.Template> col ? col.Count : -1);

            return templates.Select(t => t.ToDto());
        }

        public async Task UpdateAsync(TemplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Updating template: {Template}", item);

            var entity = await _templateRepository.FindAsync(item.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(entity, $"Template with Id {item.Id} not found.");

            entity.UpdateFromDto(item);

            await _templateRepository.UpdateAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Template updated successfully: {Template}", entity);
        }

        public async Task<IEnumerable<TemplateDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving templates for user: {UserId}", userId);
            var templates = await _templateRepository.GetByUserIdAsync(userId, cancellationToken);

            _logger.LogInformation("Retrieved {Count} templates for user {UserId}", 
                templates is ICollection<Domain.Model.Template> col ? col.Count : -1, userId);

            return templates.Select(t => t.ToDto());
        }

        public async Task AssignTemplateToUserAsync(Guid templateId, Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Assigning template {TemplateId} to user {UserId}", templateId, userId);
            
            await _templateRepository.AssignTemplateToUserAsync(templateId, userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Template {TemplateId} assigned to user {UserId} successfully", templateId, userId);
        }

        public async Task UnassignTemplateFromUserAsync(Guid templateId, Guid userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Unassigning template {TemplateId} from user {UserId}", templateId, userId);
            
            await _templateRepository.UnassignTemplateFromUserAsync(templateId, userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Template {TemplateId} unassigned from user {UserId} successfully", templateId, userId);
        }

        public async Task<IEnumerable<TemplateDto>> GetAllDeletedAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all deleted templates (admin)...");
            var templates = await _templateRepository.GetAllDeletedAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} deleted templates", templates is ICollection<Domain.Model.Template> col ? col.Count : -1);

            return templates.Select(t => t.ToDto());
        }

        public async Task<TemplateDto?> FindDeletedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Finding deleted template (admin): {Id}", id);
            var template = await _templateRepository.FindDeletedAsync(id, cancellationToken);

            if (template == null)
            {
                _logger.LogWarning("No deleted template found with Id: {Id}", id);
                return null;
            }

            _logger.LogInformation("Deleted template found: {Template}", template);
            return template.ToDto();
        }
    }
}
