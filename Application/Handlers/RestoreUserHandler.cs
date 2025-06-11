using Application.Commands;
using Application.Interfaces;
using MediatR;

namespace Application.Handlers;

public class RestoreUserHandler(IUserRepository repository) : IRequestHandler<RestoreUserCommand, bool>
{
    public async Task<bool> Handle(RestoreUserCommand request, CancellationToken cancellationToken)
    {
        var currentUser = await repository.GetByLoginAsync(request.CurrentUserLogin);
        if (currentUser is null || !currentUser.Admin)
            return false;

        var user = await repository.GetByLoginAsync(request.TargetLogin);
        if (user is null)
            return false;
        
        if (user.RevokedOn == null)
            return false;

        repository.Restore(user);
        repository.Update(user);
        await repository.SaveChangesAsync();
        return true;
    }
}