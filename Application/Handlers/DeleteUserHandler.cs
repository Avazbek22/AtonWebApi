using Application.Commands;
using Application.Interfaces;
using MediatR;

namespace Application.Handlers;

public class DeleteUserHandler(IUserRepository repository) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        // Только админы могут удалять
        var currentUser = await repository.GetByLoginAsync(request.CurrentUserLogin);
        if (currentUser is null || !currentUser.Admin)
            return false;
        
        // Нельзя удалить самого себя
        if (request.CurrentUserLogin == request.TargetLogin)
            return false;

        var user = await repository.GetByLoginAsync(request.TargetLogin);
        if (user is null)
            return false;

        if (request.HardDelete)
        {
            repository.HardDelete(user);
        }
        else
        {
            // Мягкое удаление
            repository.SoftDelete(user, request.CurrentUserLogin, DateTime.UtcNow);
            repository.Update(user);
        }

        await repository.SaveChangesAsync();
        return true;
    }
}