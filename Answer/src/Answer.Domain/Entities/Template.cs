using Answer.Domain.Common;

namespace Answer.Domain.Entities;

public class Template : BaseEntity
{
    public string Title { get; set; } = string.Empty;
}
