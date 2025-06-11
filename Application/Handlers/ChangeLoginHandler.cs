using Application.Commands;
using Application.Interfaces;
using MediatR;

namespace Application.Handlers;

public class ChangeLoginHandler(IUserRepository repository) : IRequestHandler<ChangeLoginCommand, bool>
{
    public async Task<bool> Handle(ChangeLoginCommand request, CancellationToken cancellationToken)
    {
        // 1) Получаем текущего пользователя
        var user = await repository.GetByLoginAsync(request.CurrentLogin);
        if (user is null)
            return false;

        // 2) Проверяем права (как прежде)
        var currentUser = await repository.GetByLoginAsync(request.CurrentUserLogin);
        if (currentUser is null)
            return false;
        bool isAdmin = currentUser.Admin;
        bool isSelfActive = currentUser.Login == user.Login && user.RevokedOn == null;
        if (!isAdmin && !isSelfActive)
            return false;

        // 3) Проверяем, что новый логин свободен
        var existing = await repository.GetByLoginAsync(request.NewLogin);
        if (existing is not null)
            return false;

        // 4) Меняем
        user.Login = request.NewLogin;
        user.ModifiedOn = DateTime.UtcNow;
        user.ModifiedBy = request.CurrentUserLogin;

        repository.Update(user);
        await repository.SaveChangesAsync();
        return true;
    }
}