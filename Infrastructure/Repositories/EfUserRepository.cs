using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EfUserRepository(UserDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
    }

    public async Task<User?> GetByLoginAsync(string login)
    {
        return await dbContext.Users
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<IReadOnlyList<User>> GetAllActiveOrderedByCreatedOnAsync()
    {
        return await dbContext.Users
            .Where(u => u.RevokedOn == null)
            .OrderBy(u => u.CreatedOn)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<User>> GetUsersOlderThanAsync(int age)
    {
        var cutoffDate = DateTime.UtcNow.AddYears(-age);
        return await dbContext.Users
            .Where(u => u.Birthday.HasValue && u.Birthday.Value <= cutoffDate)
            .ToListAsync();
    }

    public void SoftDelete(User user, string revokedBy, DateTime revokedOn)
    {
        user.RevokedOn = revokedOn;
        user.RevokedBy = revokedBy;
        dbContext.Users.Update(user);
    }

    public void HardDelete(User user)
    {
        dbContext.Users.Remove(user);
    }

    public void Restore(User user)
    {
        user.RevokedOn = null;
        user.RevokedBy = null;
        dbContext.Users.Update(user);
    }

    public void Update(User user)
    {
        dbContext.Users.Update(user);
    }

    public async Task SaveChangesAsync()
    {
        await dbContext.SaveChangesAsync();
    }
}