using System.Text.Json;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class TemplateService : ITemplateService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl;

        public TemplateService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _baseUrl = configuration["ApiEndpoints:TemplateService"] 
                ?? throw new InvalidOperationException("TemplateService endpoint not configured.");
        }

        public async Task<TemplateDto> CreateAsync(TemplateDto templateDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, templateDto, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TemplateDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize TemplateDto.");
        }

        public async Task DeleteAsync(Guid templateId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{templateId}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TemplateDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(_baseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<TemplateDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<TemplateDto>();
        }

        public async Task<TemplateDto> GetByIdAsync(Guid templateId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{templateId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TemplateDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Template not found.");
        }

        public async Task UpdateAsync(TemplateDto templateDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{templateDto.Id}", templateDto, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Retrieves all templates associated with a specific user by calling the Template API.
        /// Endpoint: GET /api/template/user/{userId}
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose templates to retrieve</param>
        /// <param name="cancellationToken">Cancellation token for async operation</param>
        /// <returns>A collection of TemplateDto objects owned by or accessible to the user</returns>
        /// <remarks>
        /// This method is used in the User Details view to display templates via TypeScript.
        /// The endpoint is accessed through the YARP reverse proxy at /proxy/template/user/{userId}
        /// </remarks>
        public async Task<IEnumerable<TemplateDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/user/{userId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<TemplateDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<TemplateDto>();
        }
    }
}
