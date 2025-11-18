namespace Comment.Domain.DTOs
{
    public record CommentDto
    {
        public Guid Id { get; init; }

        public TamplateDto TamplateDto { get; init; }

        public string Text { get; init; }
    }
}
