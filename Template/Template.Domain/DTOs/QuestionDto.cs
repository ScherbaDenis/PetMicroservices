using System;
using System.Text.Json.Serialization;

namespace Template.Domain.DTOs
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "questionType")]
    [JsonDerivedType(typeof(SingleLineStringQuestionDto), typeDiscriminator: "SingleLineString")]
    [JsonDerivedType(typeof(MultiLineTextQuestionDto), typeDiscriminator: "MultiLineText")]
    [JsonDerivedType(typeof(PositiveIntegerQuestionDto), typeDiscriminator: "PositiveInteger")]
    [JsonDerivedType(typeof(CheckboxQuestionDto), typeDiscriminator: "Checkbox")]
    [JsonDerivedType(typeof(BooleanQuestionDto), typeDiscriminator: "Boolean")]
    public abstract record QuestionDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
    }

    public record SingleLineStringQuestionDto : QuestionDto
    {
    }

    public record MultiLineTextQuestionDto : QuestionDto
    {
    }

    public record PositiveIntegerQuestionDto : QuestionDto
    {
    }

    public record CheckboxQuestionDto : QuestionDto
    {
        public IEnumerable<string>? Options { get; init; }
    }

    public record BooleanQuestionDto : QuestionDto
    {
    }
}
