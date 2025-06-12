using Application.Commands;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Handlers;

public class CreateUserHandler(IUserRepository repository, IMapper mapper) : IRequestHandler<CreateUserCommand, bool>
{
    public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Проверяем, что логин уникален
        var existing = await repository.GetByLoginAsync(request.Dto.Login);
        if (existing is not null)
        {
            return false; // Логин уже занят
        }
        
        var user = mapper.Map<User>(request.Dto);
        user.CreatedBy = request.CurrentUserLogin;

        await repository.AddAsync(user);
        await repository.SaveChangesAsync();
        return true;
    }
}