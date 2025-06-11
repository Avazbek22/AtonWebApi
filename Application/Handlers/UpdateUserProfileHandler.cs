using Application.Commands;
using Application.Interfaces;
using MediatR;

namespace Application.Handlers;

public class UpdateUserProfileHandler(IUserRepository repository) : IRequestHandler<UpdateUserProfileCommand, bool>
{
    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
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
        {
            return false;
        }
        
        
        if (!string.IsNullOrWhiteSpace(request.NewName))
        {
            user.Name = request.NewName;
        }
        if (request.NewGender.HasValue)
        {
            user.Gender = request.NewGender.Value;
        }
        if (request.NewBirthday.HasValue)
        {
            user.Birthday = request.NewBirthday;
        }

        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = request.CurrentUserLogin;

        repository.Update(user);
        await repository.SaveChangesAsync();
        return true;
    }
}