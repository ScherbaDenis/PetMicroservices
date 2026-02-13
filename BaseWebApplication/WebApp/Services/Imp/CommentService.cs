using System.Text.Json;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class CommentService : ICommentService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl;

        public CommentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _baseUrl = configuration["ApiEndpoints:CommentService"] 
                ?? throw new InvalidOperationException("CommentService endpoint not configured.");
        }

        public async Task<CommentDto> CreateAsync(CommentDto commentDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, commentDto, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CommentDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize CommentDto.");
        }

        public async Task DeleteAsync(Guid commentId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{commentId}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<CommentDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(_baseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<CommentDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<CommentDto>();
        }

        public async Task<CommentDto> GetByIdAsync(Guid commentId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{commentId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<CommentDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Comment not found.");
        }

        public async Task<IEnumerable<CommentDto>> GetByTemplateIdAsync(Guid templateId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/template/{templateId}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<CommentDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<CommentDto>();
        }

        public async Task UpdateAsync(CommentDto commentDto, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{commentDto.Id}", commentDto, _jsonOptions, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
