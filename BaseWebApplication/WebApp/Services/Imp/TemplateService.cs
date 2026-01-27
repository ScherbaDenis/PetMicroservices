using System.Text.Json;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class TemplateService : ITemplateService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "https://localhost:7263/api/template";

        public TemplateService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<TemplateDto> CreateAsync(TemplateDto templateDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, templateDto, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TemplateDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize TemplateDto.");
        }

        public async Task DeleteAsync(Guid templateId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{templateId}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TemplateDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(BaseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<TemplateDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<TemplateDto>();
        }

        public async Task<TemplateDto> GetByIdAsync(Guid templateId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{templateId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TemplateDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Template not found.");
        }

        public async Task UpdateAsync(TemplateDto templateDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{templateDto.Id}", templateDto, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
