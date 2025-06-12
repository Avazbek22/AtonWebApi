using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class User
{
    [Required]
    public required Guid Guid { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Логин может содержать только латинские буквы и цифры.")]
    public required string Login { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Пароль может содержать только латинские буквы и цифры.")]
    public required string Password { get; set; } = null!;

    [Required]
    [RegularExpression(@"^[a-zA-Zа-яА-ЯёЁ]+$", ErrorMessage = "Имя может содержать только буквы русского/латинского алфавита.")]
    public required string Name { get; set; } = null!;

    [Required]
    [Range(0,2, ErrorMessage = "Пол может иметь только следующие значения: 0 (женщина), 1 (мужчина) или 2 (неизвестно).")]
    public required int Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public bool Admin { get; set; }

    public required DateTime CreatedOn { get; set; }

    public required string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? RevokedOn { get; set; }

    public string? RevokedBy { get; set; }
}