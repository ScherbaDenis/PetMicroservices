using System;
using System.Text.Json.Serialization;

namespace Template.Domain.DTOs
{
    // Use custom JsonConverter for polymorphic deserialization
    [JsonConverter(typeof(QuestionDtoJsonConverter))]
    public abstract class QuestionDto
    {
        public QuestionDto()
        {
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? TemplateId { get; set; }
        
        // Explicit discriminator property for JSON serialization
        [JsonPropertyName("questionType")]
        public string QuestionType { get; set; } = string.Empty;
    }

    public class SingleLineStringQuestionDto : QuestionDto
    {
        public SingleLineStringQuestionDto()
        {
            QuestionType = "SingleLineString";
        }
    }

    public class MultiLineTextQuestionDto : QuestionDto
    {
        public MultiLineTextQuestionDto()
        {
            QuestionType = "MultiLineText";
        }
    }

    public class PositiveIntegerQuestionDto : QuestionDto
    {
        public PositiveIntegerQuestionDto()
        {
            QuestionType = "PositiveInteger";
        }
    }

    public class CheckboxQuestionDto : QuestionDto
    {
        public CheckboxQuestionDto()
        {
            QuestionType = "Checkbox";
        }

        public IEnumerable<string>? Options { get; set; }
    }

    public class BooleanQuestionDto : QuestionDto
    {
        public BooleanQuestionDto()
        {
            QuestionType = "Boolean";
        }
    }
}
