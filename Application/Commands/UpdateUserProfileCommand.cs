using MediatR;

namespace Application.Commands;

public class UpdateUserProfileCommand(string targetLogin, string? newName, int? newGender, DateTime? newBirthday, string currentUserLogin) : IRequest<bool>
{
    public string TargetLogin { get; } = targetLogin;
    public string? NewName { get; } = newName;
    public int? NewGender { get; } = newGender;
    public DateTime? NewBirthday { get; } = newBirthday;
    public string CurrentUserLogin { get; } = currentUserLogin;
}