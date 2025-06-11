using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using AutoMapper;
using MediatR;

namespace Application.Handlers;

public class GetUserByLoginHandler(IUserRepository repository, IMapper mapper) : IRequestHandler<GetUserByLoginQuery, UserViewDto?>
{
    public async Task<UserViewDto?> Handle(GetUserByLoginQuery request, CancellationToken cancellationToken)
    {
        // Только админы могут запрашивать любого пользователя
        var currentUser = await repository.GetByLoginAsync(request.CurrentUserLogin);
        if (currentUser is null || !currentUser.Admin)
            return null;

        var user = await repository.GetByLoginAsync(request.TargetLogin);
        if (user is null)
            return null;

        return mapper.Map<UserViewDto>(user);
    }
}