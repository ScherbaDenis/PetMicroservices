using System.Text.Json;
using System.Text.Json.Serialization;
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
                PropertyNameCaseInsensitive = true,
                Converters = { new QuestionDtoJsonConverter() }
            };

            _baseUrl = configuration["ApiEndpoints:CommentService"]
                ?? throw new InvalidOperationException("CommentService endpoint not configured.");
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

    // Custom JSON converter to handle Question polymorphism
    public class QuestionDtoJsonConverter : JsonConverter<QuestionDto>
    {
        public override QuestionDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            if (!root.TryGetProperty("questionType", out var typeProperty) &&
                !root.TryGetProperty("QuestionType", out typeProperty))
            {
                throw new JsonException("QuestionType property not found");
            }

            var questionType = typeProperty.GetString();
            var json = root.GetRawText();

            return questionType switch
            {
                "SingleLineString" => JsonSerializer.Deserialize<SingleLineStringQuestionDto>(json, options),
                "MultiLineText" => JsonSerializer.Deserialize<MultiLineTextQuestionDto>(json, options),
                "PositiveInteger" => JsonSerializer.Deserialize<PositiveIntegerQuestionDto>(json, options),
                "Checkbox" => JsonSerializer.Deserialize<CheckboxQuestionDto>(json, options),
                "Boolean" => JsonSerializer.Deserialize<BooleanQuestionDto>(json, options),
                _ => throw new JsonException($"Unknown question type: {questionType}")
            };
        }

        public override void Write(Utf8JsonWriter writer, QuestionDto value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
