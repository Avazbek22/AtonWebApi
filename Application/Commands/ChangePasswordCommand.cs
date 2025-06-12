using System.ComponentModel.DataAnnotations;
using MediatR;

namespace Application.Commands;

public class ChangePasswordCommand(string targetLogin, string newPassword, string currentUserLogin) : IRequest<bool>
{
    public string TargetLogin { get; } = targetLogin;
    
    [Required]
    [StringLength(32, MinimumLength = 8, ErrorMessage = "Пароль должен содержать от 8 до 100 символов.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать хотя бы одну заглавную букву, одну строчную и одну цифру.")]
    public string NewPassword { get; } = newPassword;
    public string CurrentUserLogin { get; } = currentUserLogin;
}