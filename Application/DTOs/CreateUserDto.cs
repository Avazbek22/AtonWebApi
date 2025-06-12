using System.ComponentModel.DataAnnotations;

namespace Application.DTOs;

public class CreateUserDto
{
    [Required]
    public string Login { get; set; } = null!;

    [Required]
    [StringLength(32, MinimumLength = 8, ErrorMessage = "Пароль должен содержать от 8 до 100 символов.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать хотя бы одну заглавную букву, одну строчную и одну цифру.")]
    public string Password { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    [Range(0, 2, ErrorMessage = "Пол может быть только 0, 1 или 2")]
    public int Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public bool Admin { get; set; }
}