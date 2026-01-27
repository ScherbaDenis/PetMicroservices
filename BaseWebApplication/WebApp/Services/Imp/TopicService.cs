using System.Text.Json;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class TopicService : ITopicService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "https://localhost:7263/api/topic";

        public TopicService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<TopicDto> CreateAsync(TopicDto item, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, item, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TopicDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize TopicDto.");
        }

        public async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TopicDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(BaseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<TopicDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<TopicDto>();
        }

        public async Task<TopicDto> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TopicDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Topic not found.");
        }

        public async Task UpdateAsync(TopicDto item, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{item.Id}", item, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
