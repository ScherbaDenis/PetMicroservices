using System;

namespace Template.Domain.DTOs
{
    public record QuestionDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
    }
}
