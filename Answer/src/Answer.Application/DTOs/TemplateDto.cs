namespace Answer.Application.DTOs;

public class TemplateDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class CreateTemplateDto
{
    public string Title { get; set; } = string.Empty;
}

public class UpdateTemplateDto
{
    public string Title { get; set; } = string.Empty;
}
