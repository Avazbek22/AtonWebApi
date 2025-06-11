using Application.DTOs;
using MediatR;

namespace Application.Queries;

public class GetUserByLoginQuery(string targetLogin, string currentUserLogin) : IRequest<UserViewDto?>
{
    public string TargetLogin { get; } = targetLogin;
    public string CurrentUserLogin { get; } = currentUserLogin;
}