using Microsoft.EntityFrameworkCore;

namespace EfUserReposotory.Tests
{
    // Сущность User и DbContext для тестов
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int Age { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }

    // Реализация репозитория для тестирования
    public class EfUserRepository(AppDbContext context)
    {
        public void Add(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }

        public void Update(User user)
        {
            context.Users.Update(user);
            context.SaveChanges();
        }

        public void SoftDelete(int userId)
        {
            var user = context.Users.Find(userId);
            if (user != null)
            {
                user.IsActive = false;
                context.SaveChanges();
            }
        }

        public void Restore(int userId)
        {
            var user = context.Users.Find(userId);
            if (user != null)
            {
                user.IsActive = true;
                context.SaveChanges();
            }
        }

        public User GetById(int userId) => context.Users.Find(userId)!;

        public User GetByLogin(string login) => context.Users.FirstOrDefault(u => u.Login == login)!;

        public IEnumerable<User> GetAll() => context.Users.ToList();

        public IEnumerable<User> GetActiveUsers() => context.Users.Where(u => u.IsActive).ToList();

        public IEnumerable<User> FilterByName(string substring) =>
            context.Users
                .Where(u => u.Name.ToLower().Contains(substring.ToLower()))
                .ToList();


        public IEnumerable<User> SortByAge() => context.Users.OrderBy(u => u.Age).ToList();
    }

    public class EfUserRepositoryTests
    {
        private DbContextOptions<AppDbContext> GetInMemoryOptions(string dbName)
        {
            return new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
        }

        [Fact]
        public void AddUser_ShouldAddUser()
        {
            var options = GetInMemoryOptions("AddUserDb");
            using (var context = new AppDbContext(options))
            {
                var repo = new EfUserRepository(context);
                var user = new User { Name = "Alice", Login = "alice", Password = "pass", Age = 30 };
                repo.Add(user);

                Assert.Equal(1, context.Users.Count());
                var added = context.Users.FirstOrDefault();
                Assert.NotNull(added);
                Assert.Equal("alice", added.Login);
            }
        }

        [Fact]
        public void UpdateUser_ShouldUpdateExistingUser()
        {
            var options = GetInMemoryOptions("UpdateUserDb");
            using (var context = new AppDbContext(options))
            {
                context.Users.Add(new User { Name = "Bob", Login = "bob", Password = "pass", Age = 25 });
                context.SaveChanges();

                var repo = new EfUserRepository(context);
                var user = context.Users.First();
                user.Name = "Robert";
                repo.Update(user);

                var updated = context.Users.Find(user.Id);
                Assert.Equal("Robert", updated!.Name);
            }
        }

        [Fact]
        public void SoftDelete_ShouldMarkUserInactive()
        {
            var options = GetInMemoryOptions("SoftDeleteDb");
            using (var context = new AppDbContext(options))
            {
                context.Users.Add(new User { Name = "Carol", Login = "carol", Password = "pass", Age = 40 });
                context.SaveChanges();

                var repo = new EfUserRepository(context);
                var userId = context.Users.First().Id;
                repo.SoftDelete(userId);

                var user = context.Users.Find(userId);
                Assert.False(user!.IsActive);
            }
        }

        [Fact]
        public void Restore_ShouldMakeUserActive()
        {
            var options = GetInMemoryOptions("RestoreDb");
            using (var context = new AppDbContext(options))
            {
                context.Users.Add(new User { Name = "Dave", Login = "dave", Password = "pass", Age = 50, IsActive = false });
                context.SaveChanges();

                var repo = new EfUserRepository(context);
                var userId = context.Users.First().Id;
                repo.Restore(userId);

                var user = context.Users.Find(userId);
                Assert.True(user!.IsActive);
            }
        }

        [Fact]
        public void FilterByName_ShouldReturnMatchingUsers()
        {
            var options = GetInMemoryOptions("FilterDb");
            using (var context = new AppDbContext(options))
            {
                context.Users.Add(new User { Name = "Eve", Login = "eve", Password = "pass", Age = 20 });
                context.Users.Add(new User { Name = "Steve", Login = "steve", Password = "pass", Age = 30 });
                context.Users.Add(new User { Name = "Alice", Login = "alice", Password = "pass", Age = 25 });
                context.SaveChanges();

                var repo = new EfUserRepository(context);
                var results = repo.FilterByName("eve").ToList();

                Assert.Equal(2, results.Count);
                Assert.Contains(results, u => u.Name == "Eve");
                Assert.Contains(results, u => u.Name == "Steve");
            }
        }

        [Fact]
        public void GetActiveUsers_ShouldReturnOnlyActive()
        {
            var options = GetInMemoryOptions("ActiveDb");
            using (var context = new AppDbContext(options))
            {
                context.Users.Add(new User { Name = "ActiveUser", Login = "active", Password = "pass", Age = 22, IsActive = true });
                context.Users.Add(new User { Name = "InactiveUser", Login = "inactive", Password = "pass", Age = 22, IsActive = false });
                context.SaveChanges();

                var repo = new EfUserRepository(context);
                var activeUsers = repo.GetActiveUsers().ToList();

                Assert.Single(activeUsers);
                Assert.Equal("active", activeUsers.First().Login);
            }
        }

        [Fact]
        public void SortByAge_ShouldReturnUsersInAscendingOrder()
        {
            var options = GetInMemoryOptions("SortDb");
            using (var context = new AppDbContext(options))
            {
                context.Users.Add(new User { Name = "User1", Login = "u1", Password = "pass", Age = 50 });
                context.Users.Add(new User { Name = "User2", Login = "u2", Password = "pass", Age = 20 });
                context.Users.Add(new User { Name = "User3", Login = "u3", Password = "pass", Age = 35 });
                context.SaveChanges();

                var repo = new EfUserRepository(context);
                var sorted = repo.SortByAge().ToList();

                Assert.Equal(20, sorted[0].Age);
                Assert.Equal(35, sorted[1].Age);
                Assert.Equal(50, sorted[2].Age);
            }
        }
    }
}
