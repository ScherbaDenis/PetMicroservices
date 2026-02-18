using System.Text.Json;
using System.Text.Json.Serialization;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class AnswerService : IAnswerService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl;

        public AnswerService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter() }
            };

            _baseUrl = configuration["ApiEndpoints:AnswerService"]
                ?? throw new InvalidOperationException("AnswerService endpoint not configured.");
        }

        public async Task<AnswerDto> CreateAsync(CreateAnswerDto answerDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, answerDto, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AnswerDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize AnswerDto.");
        }

        public async Task DeleteAsync(Guid answerId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{answerId}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<AnswerDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(_baseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<AnswerDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<AnswerDto>();
        }

        public async Task<AnswerDto?> GetByIdAsync(Guid answerId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{answerId}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AnswerDto>(_jsonOptions, cancellationToken);
        }

        public async Task UpdateAsync(UpdateAnswerDto answerDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{answerDto.Id}", answerDto, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
