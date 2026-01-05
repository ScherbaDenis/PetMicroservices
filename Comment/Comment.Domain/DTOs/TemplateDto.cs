namespace Comment.Domain.DTOs
{
    public record TemplateDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
    }
}
