using Comment.Domain.DTOs;

namespace Comment.Domain.Services
{
    /// <summary>
    /// Service contract for template operations using DTOs.
    /// </summary>
    public interface ITemplateService : IService<TemplateDto, Guid>
    {
    }
}
