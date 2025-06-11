using Application.DTOs;
using MediatR;

namespace Application.Queries;

public class GetAllActiveUsersQuery(string currentUserLogin) : IRequest<IReadOnlyList<UserViewDto>>
{
    public string CurrentUserLogin { get; } = currentUserLogin;
}