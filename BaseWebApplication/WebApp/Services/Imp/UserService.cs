using System.Text.Json;
using WebApp.Services.DTOs;

namespace WebApp.Services.Imp
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private const string BaseUrl = "https://localhost:7263/api/user";

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<UserDto> CreateAsync(UserDto item, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, item, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("Failed to deserialize UserDto.");
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(BaseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>(_jsonOptions, cancellationToken)
                   ?? Enumerable.Empty<UserDto>();
        }

        public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions, cancellationToken)
                   ?? throw new InvalidOperationException("User not found.");
        }

        public async Task UpdateAsync(UserDto item, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{item.Id}", item, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
