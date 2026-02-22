using Template.Domain.Repository;

namespace Template.Domain.Model
{
    public abstract class Question : Entity<Guid>
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? TemplateId { get; set; }
    }

    public class SingleLineStringQuestion : Question
    {
    }

    public class MultiLineTextQuestion : Question
    {
    }

    public class PositiveIntegerQuestion : Question
    {
    }

    public class CheckboxQuestion : Question
    {
        public IEnumerable<string>? Options { get; set; }
    }

    public class BooleanQuestion : Question
    {
    }
}
