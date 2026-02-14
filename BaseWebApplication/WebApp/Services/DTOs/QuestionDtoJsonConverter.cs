using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApp.Services.DTOs
{
    public class QuestionDtoJsonConverter : JsonConverter<QuestionDto>
    {
        public override QuestionDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;

            // Read the questionType discriminator
            if (!root.TryGetProperty("questionType", out var typeProperty))
            {
                throw new JsonException("QuestionType property not found in JSON");
            }

            var questionType = typeProperty.GetString();

            // Deserialize based on the questionType
            return questionType switch
            {
                "SingleLineString" => JsonSerializer.Deserialize<SingleLineStringQuestionDto>(root.GetRawText(), options),
                "MultiLineText" => JsonSerializer.Deserialize<MultiLineTextQuestionDto>(root.GetRawText(), options),
                "PositiveInteger" => JsonSerializer.Deserialize<PositiveIntegerQuestionDto>(root.GetRawText(), options),
                "Checkbox" => JsonSerializer.Deserialize<CheckboxQuestionDto>(root.GetRawText(), options),
                "Boolean" => JsonSerializer.Deserialize<BooleanQuestionDto>(root.GetRawText(), options),
                _ => throw new JsonException($"Unknown question type: {questionType}")
            };
        }

        public override void Write(Utf8JsonWriter writer, QuestionDto value, JsonSerializerOptions options)
        {
            // Use default serialization which will include the QuestionType property
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
