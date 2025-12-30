namespace Comment.Domain.DTOs
{
    public record TamplateDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
    }
}
