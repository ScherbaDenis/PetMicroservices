using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class TemplateService : ITemplateService
    {
        public Task<TemplateDto> CreateAsync(TemplateDto templateDto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid templateId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TemplateDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TemplateDto> GetByIdAsync(Guid templateId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TemplateDto templateDto, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
