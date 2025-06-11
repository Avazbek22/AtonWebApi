using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using AutoMapper;
using MediatR;

namespace Application.Handlers;

public class GetAllActiveUsersHandler(IUserRepository repository, IMapper mapper)
    : IRequestHandler<GetAllActiveUsersQuery, IReadOnlyList<UserViewDto>>
{
    public async Task<IReadOnlyList<UserViewDto>> Handle(GetAllActiveUsersQuery request, CancellationToken cancellationToken)
    {
        // Проверяем — только админы могут смотреть всех активных
        var currentUser = await repository.GetByLoginAsync(request.CurrentUserLogin);
        if (currentUser is null || !currentUser.Admin)
            return new List<UserViewDto>();

        var users = await repository.GetAllActiveOrderedByCreatedOnAsync();
        
        return mapper.Map<List<UserViewDto>>(users);
    }
}