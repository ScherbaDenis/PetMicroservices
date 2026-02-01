using WebApp.Services.DTOs;

namespace WebApp.Services
{
    /// <summary>
    /// Service interface for managing templates
    /// </summary>
    public interface ITemplateService
    {
        Task<TemplateDto> GetByIdAsync(Guid templateId, CancellationToken cancellationToken);
        Task<TemplateDto> CreateAsync(TemplateDto templateDto, CancellationToken cancellationToken);
        Task DeleteAsync(Guid templateId, CancellationToken cancellationToken);
        Task UpdateAsync(TemplateDto templateDto, CancellationToken cancellationToken);
        Task<IEnumerable<TemplateDto>> GetAllAsync(CancellationToken cancellationToken);
        
        /// <summary>
        /// Retrieves all templates associated with a specific user.
        /// This method calls the Template microservice API endpoint: GET /api/template/user/{userId}
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>A collection of templates associated with the user</returns>
        /// <example>
        /// Usage in a controller:
        /// <code>
        /// var userTemplates = await _templateService.GetByUserIdAsync(userId, cancellationToken);
        /// </code>
        /// </example>
        Task<IEnumerable<TemplateDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    }
}
