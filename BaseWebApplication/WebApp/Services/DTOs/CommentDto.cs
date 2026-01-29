namespace WebApp.Services.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; init; }
        public TemplateDto? Template { get; init; }
        public string Text { get; init; } = string.Empty;
    }
}
