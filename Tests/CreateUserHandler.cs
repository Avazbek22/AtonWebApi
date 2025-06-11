using Moq;

namespace Create.Tests
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public interface IUserRepository
    {
        User GetByLogin(string login);
        void Add(User user);
    }

    public class CreateUserCommand { public string Login; public string Name; public string Password; }
    public class CreateUserHandler(IUserRepository repo)
    {
        public async Task<int> Handle(CreateUserCommand cmd)
        {
            if (string.IsNullOrEmpty(cmd.Login) || string.IsNullOrEmpty(cmd.Password))
                throw new ArgumentException("Invalid input");
            if (repo.GetByLogin(cmd.Login) != null!)
                throw new InvalidOperationException("Login already exists");
            var user = new User { Login = cmd.Login, Name = cmd.Name, Password = cmd.Password };
            repo.Add(user);
            return await Task.FromResult(user.Id);
        }
    }

    public class CreateUserHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldCreateUser_WhenDataValid()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByLogin("john")).Returns((User)null!);
            var handler = new CreateUserHandler(mockRepo.Object);
            var cmd = new CreateUserCommand { Login = "john", Name = "John Doe", Password = "123456" };

            await handler.Handle(cmd);

            mockRepo.Verify(r => r.Add(It.Is<User>(u => u.Login == "john")), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenLoginExists()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByLogin("john")).Returns(new User { Login = "john" });
            var handler = new CreateUserHandler(mockRepo.Object);
            var cmd = new CreateUserCommand { Login = "john", Name = "John", Password = "123" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenLoginEmpty()
        {
            var mockRepo = new Mock<IUserRepository>();
            var handler = new CreateUserHandler(mockRepo.Object);
            var cmd = new CreateUserCommand { Login = "", Name = "NoLogin", Password = "123" };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
        }
    }
}
