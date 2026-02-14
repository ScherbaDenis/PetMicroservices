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
        public QuestionDto()
        {
        }

        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string QuestionType { get; set; } = string.Empty; // For UI rendering
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

        public IEnumerable<string>? Options { get; set; }
    }

    public class BooleanQuestionDto : QuestionDto
    {
        public BooleanQuestionDto()
        {
        }
    }
}
