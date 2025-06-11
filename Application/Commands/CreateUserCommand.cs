using Application.DTOs;
using MediatR;

namespace Application.Commands;

public class CreateUserCommand(CreateUserDto dto, string currentUserLogin) : IRequest<bool>
{
    public CreateUserDto Dto { get; } = dto;

    public string CurrentUserLogin { get; } = currentUserLogin;
}