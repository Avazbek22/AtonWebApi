using Application.DTOs;
using MediatR;

namespace Application.Queries;

public class AuthenticateUserQuery(string login, string password) : IRequest<UserViewDto?>
{
    public string Login { get; } = login;
    public string Password { get; } = password;
}