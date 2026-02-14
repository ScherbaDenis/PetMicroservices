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
        protected QuestionDto()
        {
        }

        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string QuestionType { get; init; } = string.Empty; // For UI rendering
    }

    public class SingleLineStringQuestionDto : QuestionDto
    {
        public SingleLineStringQuestionDto()
        {
        }
    }

    public class MultiLineTextQuestionDto : QuestionDto
    {
        public MultiLineTextQuestionDto()
        {
        }
    }

    public class PositiveIntegerQuestionDto : QuestionDto
    {
        public PositiveIntegerQuestionDto()
        {
        }
    }

    public class CheckboxQuestionDto : QuestionDto
    {
        public CheckboxQuestionDto()
        {
        }

        public IEnumerable<string>? Options { get; init; }
    }

    public class BooleanQuestionDto : QuestionDto
    {
        public BooleanQuestionDto()
        {
        }
    }
}
