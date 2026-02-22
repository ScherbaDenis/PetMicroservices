using Comment.Domain.DTOs;
using Comment.Domain.Mappers;

using Microsoft.Extensions.Logging;
using Comment.Domain.Repositories;
using System.Linq.Expressions;

namespace Comment.Service.Services
{
    public class TemplateService(IUnitOfWork unitOfWork, ILogger<TemplateService> logger) : ITemplateService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        private readonly ILogger<TemplateService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ITemplateRepository _templateRepository = unitOfWork.TemplateRepository;

        public async Task CreateAsync(TemplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Creating template: {Template}", item);

            var entity = item.ToEntity();
            await _templateRepository.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Template created successfully: {Template}", entity);
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

        public IEnumerable<TemplateDto> Find(Func<TemplateDto, bool> predicate)
        {
            _logger.LogInformation("Finding template...");
            var entities = _templateRepository.GetAllAsync().Result.Where(t => predicate(t.ToDto()));
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

        public async Task<IEnumerable<TemplateDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all templates...");
            var templates = await _templateRepository.GetAllAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} templates", templates is ICollection<Domain.Models.Template> col ? col.Count : -1);

            return templates.Select(t => t.ToDto());
        }

        public async Task UpdateAsync(TemplateDto item, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(item);
            _logger.LogInformation("Updating template: {Template}", item);

            var dbEntity = await _templateRepository.FindAsync(item.Id, cancellationToken);
            ArgumentNullException.ThrowIfNull(dbEntity, $"Template with Id {item.Id} not found.");

            dbEntity.Title = item.Title; // Todo: Map other properties as needed

            await _templateRepository.UpdateAsync(dbEntity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Template updated successfully: {Template}", dbEntity);
        }
        public async Task<IEnumerable<TemplateDto>> FindAsync(Expression<Func<TemplateDto, bool>> predicate, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            _logger.LogInformation("Finding templates with predicate...");

            var entities = await _templateRepository.FindAsync(t => predicate.Compile()(t.ToDto()), cancellationToken);
            return entities.Select(t => t.ToDto());
        }

        public async Task<(IEnumerable<TemplateDto> Items, int TotalCount)> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<TemplateDto, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize <= 0) throw new ArgumentOutOfRangeException(nameof(pageSize));

            _logger.LogInformation("Retrieving paged templates: PageIndex={PageIndex}, PageSize={PageSize}", pageIndex, pageSize);

            var (items, totalCount) = await _templateRepository.GetPagedAsync(
                pageIndex,
                pageSize,
                predicate != null ? t => predicate.Compile()(t.ToDto()) : null,
                cancellationToken
            );

            return (items.Select(t => t.ToDto()), totalCount);
        }

        public async Task<IEnumerable<TemplateDto>> GetAllDeletedAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Retrieving all deleted templates (admin)...");
            var templates = await _templateRepository.GetAllDeletedAsync(cancellationToken);

            _logger.LogInformation("Retrieved {Count} deleted templates", templates is ICollection<Domain.Models.Template> col ? col.Count : -1);

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
