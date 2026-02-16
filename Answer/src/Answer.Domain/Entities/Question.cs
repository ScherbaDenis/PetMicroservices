using Answer.Domain.Common;

namespace Answer.Domain.Entities;

public class Question : BaseEntity
{
    public string Title { get; set; } = string.Empty;
}
