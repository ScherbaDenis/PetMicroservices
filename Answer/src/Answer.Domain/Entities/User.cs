using Answer.Domain.Common;

namespace Answer.Domain.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = string.Empty;
}
