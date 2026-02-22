namespace Answer.Application.DTOs;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class CreateQuestionDto
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateQuestionDto
{
    public string Title { get; set; } = string.Empty;
}
