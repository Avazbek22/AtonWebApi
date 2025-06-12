using Moq;

namespace ChangeLoginHandler.Tests
{
    public class User { public int Id { get; set; } public string Login { get; set; } }

    public interface IUserRepository
    {
        User GetById(int id);
        User GetByLogin(string login);
        void Update(User user);
    }

    public class ChangeLoginCommand { public int Id; public string NewLogin; }
    public class ChangeLoginHandler(IUserRepository repo)
    {
        public async Task Handle(ChangeLoginCommand cmd)
        {
            var user = repo.GetById(cmd.Id);
            if (user == null) throw new KeyNotFoundException("User not found");
            if (string.IsNullOrEmpty(cmd.NewLogin)) throw new ArgumentException("Invalid login");
            if (repo.GetByLogin(cmd.NewLogin) != null!) throw new InvalidOperationException("Login already exists");
            user.Login = cmd.NewLogin;
            repo.Update(user);
            await Task.CompletedTask;
        }
    }

    public class ChangeLoginHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldChangeLogin_WhenNewLoginAvailable()
        {
            var user = new User { Id = 1, Login = "old" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(1)).Returns(user);
            mockRepo.Setup(r => r.GetByLogin("new")).Returns((User)null!);
            var handler = new ChangeLoginHandler(mockRepo.Object);
            var cmd = new ChangeLoginCommand { Id = 1, NewLogin = "new" };

            await handler.Handle(cmd);

            Assert.Equal("new", user.Login);
            mockRepo.Verify(r => r.Update(user), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(2)).Returns((User)null!);
            var handler = new ChangeLoginHandler(mockRepo.Object);
            var cmd = new ChangeLoginCommand { Id = 2, NewLogin = "login" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenNewLoginExists()
        {
            var user = new User { Id = 1, Login = "old" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(1)).Returns(user);
            mockRepo.Setup(r => r.GetByLogin("existing")).Returns(new User { Login = "existing" });
            var handler = new ChangeLoginHandler(mockRepo.Object);
            var cmd = new ChangeLoginCommand { Id = 1, NewLogin = "existing" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenNewLoginEmpty()
        {
            var user = new User { Id = 1, Login = "old" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(1)).Returns(user);
            var handler = new ChangeLoginHandler(mockRepo.Object);
            var cmd = new ChangeLoginCommand { Id = 1, NewLogin = "" };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }
    }
}
