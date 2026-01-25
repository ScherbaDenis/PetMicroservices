using System.Text.Json;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class TagService : ITagService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "https://localhost:7263/api/tag";

        public TagService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<TagDto> CreateAsync(TagDto item, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, item, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TagDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize TagDto.");
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TagDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(BaseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<TagDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<TagDto>();
        }

        public async Task<TagDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TagDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Tag not found.");
        }

        public async Task UpdateAsync(TagDto item, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{item.Id}", item, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
