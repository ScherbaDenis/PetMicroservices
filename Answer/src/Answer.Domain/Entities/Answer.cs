using Answer.Domain.Common;

namespace Answer.Domain.Entities;

public class Answer : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public Guid QuestionId { get; set; }
    public Question Question { get; set; } = null!;
    
    public Guid TemplateId { get; set; }
    public Template Template { get; set; } = null!;
    
    public AnswerType AnswerType { get; set; }
    
    // Store the answer value as string - will be parsed based on AnswerType
    public string AnswerValue { get; set; } = string.Empty;
}
