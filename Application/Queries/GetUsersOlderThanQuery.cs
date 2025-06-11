using Application.DTOs;
using MediatR;

namespace Application.Queries;

/// <summary>
/// Запрос: получить всех пользователей старше указанного возраста
/// </summary>
public class GetUsersOlderThanQuery(int age, string currentUserLogin) : IRequest<IReadOnlyList<UserViewDto>>
{
    public int Age { get; } = age;
    public string CurrentUserLogin { get; } = currentUserLogin;
}