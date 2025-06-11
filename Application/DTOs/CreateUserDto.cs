namespace Application.DTOs;

public class CreateUserDto
{
    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public bool Admin { get; set; }
}