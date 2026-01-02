namespace Comment.Domain.DTOs
{
    public record CommentDto
    {
        public Guid Id { get; init; }

        public TemplateDto TemplateDto { get; init; }

        public string Text { get; init; }
    }
}
