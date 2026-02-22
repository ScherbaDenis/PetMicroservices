using Answer.Domain.Entities;

namespace Answer.Application.DTOs;

public class AnswerDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid QuestionId { get; set; }
    public string QuestionTitle { get; set; } = string.Empty;
    public Guid TemplateId { get; set; }
    public string TemplateTitle { get; set; } = string.Empty;
    public AnswerType AnswerType { get; set; }
    public string AnswerValue { get; set; } = string.Empty;
}

public class CreateAnswerDto
{
    public Guid UserId { get; set; }
    public Guid QuestionId { get; set; }
    public Guid TemplateId { get; set; }
    public AnswerType AnswerType { get; set; }
    public string AnswerValue { get; set; } = string.Empty;
}

public class UpdateAnswerDto
{
    public AnswerType AnswerType { get; set; }
    public string AnswerValue { get; set; } = string.Empty;
}
