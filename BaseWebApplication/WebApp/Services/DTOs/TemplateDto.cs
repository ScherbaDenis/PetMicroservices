namespace WebApp.Services.DTOs
{
    public class TemplateDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public UserDto? Owner { get; init; }
        public TopicDto? Topic { get; init; }
        public IEnumerable<TagDto>? Tags { get; init; }
        //public IEnumerable<QuestionDto>? Questions { get; init; }
    }
}
