using System.Text.Json;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _baseUrl;

        public UserService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _baseUrl = configuration["ApiEndpoints:UserService"] 
                ?? throw new InvalidOperationException("UserService endpoint not configured.");
        }

        public async Task<UserDto> CreateAsync(UserDto item, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, item, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize UserDto.");
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(_baseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<UserDto>();
        }

        public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("User not found.");
        }

        public async Task UpdateAsync(UserDto item, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{item.Id}", item, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
