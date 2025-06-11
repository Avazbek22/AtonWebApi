using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using AutoMapper;
using MediatR;

namespace Application.Handlers;

public class AuthenticateUserHandler(IUserRepository repository, IMapper mapper) : IRequestHandler<AuthenticateUserQuery, UserViewDto?>
{
    public async Task<UserViewDto?> Handle(AuthenticateUserQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByLoginAsync(request.Login);
        if (user is null)
            return null;
        
        if (user.Password != request.Password || user.RevokedOn != null)
            return null;

        return mapper.Map<UserViewDto>(user);
    }
}