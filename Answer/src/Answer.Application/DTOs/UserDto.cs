namespace Answer.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    public string Name { get; set; } = string.Empty;
}
