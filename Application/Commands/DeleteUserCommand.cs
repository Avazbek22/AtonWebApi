using MediatR;

namespace Application.Commands;

public class DeleteUserCommand(string targetLogin, bool hardDelete, string currentUserLogin) : IRequest<bool>
{
    public string TargetLogin { get; } = targetLogin;
    public bool HardDelete { get; } = hardDelete;
    public string CurrentUserLogin { get; } = currentUserLogin;
}