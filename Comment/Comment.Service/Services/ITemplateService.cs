using Comment.Domain.DTOs;

namespace Comment.Service.Services
{
    /// <summary>
    /// Service contract for template operations using DTOs.
    /// </summary>
    public interface ITemplateService : IService<TemplateDto, Guid>
    {
    }
}
