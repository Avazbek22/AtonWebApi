using Moq;

namespace AuthenticateUserHandler.Tests
{
    public class User { public int Id { get; set; } public string Login { get; set; } public string Password { get; set; } public bool IsActive { get; set; } = true; }
    public interface IUserRepository { User GetByLogin(string login); }

    public class AuthenticateUserCommand { public string Login; public string Password; }
    public class AuthenticateUserHandler(IUserRepository repo)
    {
        public async Task<int> Handle(AuthenticateUserCommand cmd)
        {
            var user = repo.GetByLogin(cmd.Login);
            if (user == null || user.Password != cmd.Password) throw new UnauthorizedAccessException("Invalid credentials");
            if (!user.IsActive) throw new InvalidOperationException("User not active");
            return await Task.FromResult(user.Id);
        }
    }

    public class AuthenticateUserHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnUserId_WhenCredentialsValid()
        {
            var user = new User { Id = 1, Login = "user", Password = "pass", IsActive = true };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByLogin("user")).Returns(user);
            var handler = new AuthenticateUserHandler(mockRepo.Object);
            var cmd = new AuthenticateUserCommand { Login = "user", Password = "pass" };

            var result = await handler.Handle(cmd);

            Assert.Equal(1, result);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenPasswordInvalid()
        {
            var user = new User { Id = 1, Login = "user", Password = "pass", IsActive = true };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByLogin("user")).Returns(user);
            var handler = new AuthenticateUserHandler(mockRepo.Object);
            var cmd = new AuthenticateUserCommand { Login = "user", Password = "wrong" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(cmd));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByLogin("nope")).Returns((User)null!);
            var handler = new AuthenticateUserHandler(mockRepo.Object);
            var cmd = new AuthenticateUserCommand { Login = "nope", Password = "pass" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(cmd));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNotActive()
        {
            var user = new User { Id = 2, Login = "user2", Password = "pass", IsActive = false };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByLogin("user2")).Returns(user);
            var handler = new AuthenticateUserHandler(mockRepo.Object);
            var cmd = new AuthenticateUserCommand { Login = "user2", Password = "pass" };

            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(cmd));
        }
    }
}
