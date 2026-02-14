using System.Text.Json;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class QuestionService : IQuestionService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl;

        public QuestionService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true
            };

            _baseUrl = configuration["ApiEndpoints:QuestionService"]
                ?? configuration["ApiEndpoints:TemplateService"]?.TrimEnd('/') + "/questions"
                ?? throw new InvalidOperationException("QuestionService endpoint not configured.");
        }

        public async Task<QuestionDto> CreateAsync(QuestionDto questionDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, questionDto, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QuestionDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize QuestionDto.");
        }

        public async Task DeleteAsync(Guid questionId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{questionId}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<QuestionDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(_baseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<QuestionDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<QuestionDto>();
        }

        public async Task<QuestionDto?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{questionId}", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<QuestionDto>(_jsonOptions, cancellationToken);
        }

        public async Task UpdateAsync(QuestionDto questionDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{questionDto.Id}", questionDto, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
