using MediatR;

namespace Application.Commands;

public class ChangeLoginCommand(string currentLogin, string newLogin, string currentUserLogin) : IRequest<bool>
{
    public string CurrentLogin { get; } = currentLogin;
    public string NewLogin { get; } = newLogin;
    public string CurrentUserLogin { get; } = currentUserLogin;
}