using Domain.Entities;

namespace Application.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);

    void Update(User user);

    Task<User?> GetByLoginAsync(string login);

    Task<IReadOnlyList<User>> GetAllActiveOrderedByCreatedOnAsync();

    Task<IReadOnlyList<User>> GetUsersOlderThanAsync(int age);

    void SoftDelete(User user, string revokedBy, DateTime revokedOn);

    void HardDelete(User user);

    void Restore(User user);

    Task SaveChangesAsync();
}