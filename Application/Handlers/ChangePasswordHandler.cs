using Application.Commands;
using Application.Interfaces;
using MediatR;

namespace Application.Handlers;

public class ChangePasswordHandler(IUserRepository repository) : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByLoginAsync(request.TargetLogin);
        if (user is null)
            return false;

        var currentUser = await repository.GetByLoginAsync(request.CurrentUserLogin);
        if (currentUser is null)
            return false;

        bool isAdmin = currentUser.Admin;
        bool isSelfActive = currentUser.Login == user.Login && user.RevokedOn == null;
        if (!isAdmin && !isSelfActive)
            return false;

        user.Password = request.NewPassword;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = request.CurrentUserLogin;

        repository.Update(user);
        await repository.SaveChangesAsync();
        return true;
    }
}