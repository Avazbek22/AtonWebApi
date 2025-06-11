using MediatR;

namespace Application.Commands;

public class ChangePasswordCommand(string targetLogin, string newPassword, string currentUserLogin) : IRequest<bool>
{
    public string TargetLogin { get; } = targetLogin;
    public string NewPassword { get; } = newPassword;
    public string CurrentUserLogin { get; } = currentUserLogin;
}