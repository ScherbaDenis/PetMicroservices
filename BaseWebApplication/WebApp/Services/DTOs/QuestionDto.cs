using System.Text.Json.Serialization;

namespace WebApp.Services.DTOs
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "questionType")]
    [JsonDerivedType(typeof(SingleLineStringQuestionDto), typeDiscriminator: "SingleLineString")]
    [JsonDerivedType(typeof(MultiLineTextQuestionDto), typeDiscriminator: "MultiLineText")]
    [JsonDerivedType(typeof(PositiveIntegerQuestionDto), typeDiscriminator: "PositiveInteger")]
    [JsonDerivedType(typeof(CheckboxQuestionDto), typeDiscriminator: "Checkbox")]
    [JsonDerivedType(typeof(BooleanQuestionDto), typeDiscriminator: "Boolean")]
    public abstract class QuestionDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string QuestionType { get; init; } = string.Empty; // For UI rendering
    }

    public class SingleLineStringQuestionDto : QuestionDto
    {
    }

    public class MultiLineTextQuestionDto : QuestionDto
    {
    }

    public class PositiveIntegerQuestionDto : QuestionDto
    {
    }

    public class CheckboxQuestionDto : QuestionDto
    {
        public IEnumerable<string>? Options { get; init; }
    }

    public class BooleanQuestionDto : QuestionDto
    {
    }
}
