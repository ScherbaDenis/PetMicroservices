using WebApp.Services.DTOs;

namespace WebApp.Services
{
    public interface ITemplateService
    {
        Task<TemplateDto> GetByIdAsync(Guid templateId, CancellationToken cancellationToken);
        Task<TemplateDto> CreateAsync(TemplateDto templateDto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid templateId, CancellationToken cancellationToken);
        Task UpdateAsync(TemplateDto templateDto, CancellationToken cancellationToken);
        Task<IEnumerable<TemplateDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<TemplateDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
