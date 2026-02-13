using System;

namespace Template.Domain.DTOs
{
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
