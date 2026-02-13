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
    }

    public record BooleanQuestionDto : QuestionDto
    {
    }
}
