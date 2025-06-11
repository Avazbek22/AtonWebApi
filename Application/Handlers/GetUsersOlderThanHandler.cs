using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using AutoMapper;
using MediatR;

namespace Application.Handlers;

public class GetUsersOlderThanHandler(IUserRepository repository, IMapper mapper)
    : IRequestHandler<GetUsersOlderThanQuery, IReadOnlyList<UserViewDto>>
{
    public async Task<IReadOnlyList<UserViewDto>> Handle(GetUsersOlderThanQuery request, CancellationToken cancellationToken)
    {
        var currentUser = await repository.GetByLoginAsync(request.CurrentUserLogin);
        if (currentUser is null || !currentUser.Admin)
            return new List<UserViewDto>();

        var users = await repository.GetUsersOlderThanAsync(request.Age);

        return mapper.Map<List<UserViewDto>>(users);
    }
}