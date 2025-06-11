using MediatR;

namespace Application.Commands;

public class RestoreUserCommand(string targetLogin, string currentUserLogin) : IRequest<bool>
{
    public string TargetLogin { get; } = targetLogin;
    public string CurrentUserLogin { get; } = currentUserLogin;
}