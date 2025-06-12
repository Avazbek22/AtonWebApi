using Moq;

namespace ChangePasswordHandler.Tests
{
    public class User
    {
        public int Id { get; set; } 
        public string Password { get; set; } = null!;
    }

    public interface IUserRepository
    {
        User GetById(int id);
        void Update(User user);
    }

    public class ChangePasswordCommand
    {
        public int Id; 
        public string OldPassword = null!; 
        public string NewPassword = null!;
    }
    public class ChangePasswordHandler(IUserRepository repo)
    {
        public async Task Handle(ChangePasswordCommand cmd)
        {
            var user = repo.GetById(cmd.Id);
            if (user == null) throw new KeyNotFoundException("User not found");
            if (user.Password != cmd.OldPassword) throw new UnauthorizedAccessException("Wrong password");
            if (string.IsNullOrEmpty(cmd.NewPassword)) throw new ArgumentException("Invalid password");
            user.Password = cmd.NewPassword;
            repo.Update(user);
            await Task.CompletedTask;
        }
    }

    public class ChangePasswordHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldChangePassword_WhenOldPasswordMatches()
        {
            var user = new User { Id = 1, Password = "oldPass" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(1)).Returns(user);
            var handler = new ChangePasswordHandler(mockRepo.Object);
            var cmd = new ChangePasswordCommand { Id = 1, OldPassword = "oldPass", NewPassword = "newPass" };

            await handler.Handle(cmd);

            Assert.Equal("newPass", user.Password);
            mockRepo.Verify(r => r.Update(user), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(3)).Returns((User)null!);
            var handler = new ChangePasswordHandler(mockRepo.Object);
            var cmd = new ChangePasswordCommand { Id = 3, OldPassword = "pass", NewPassword = "new" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenOldPasswordIncorrect()
        {
            var user = new User { Id = 1, Password = "correct" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(1)).Returns(user);
            var handler = new ChangePasswordHandler(mockRepo.Object);
            var cmd = new ChangePasswordCommand { Id = 1, OldPassword = "wrong", NewPassword = "new" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenNewPasswordEmpty()
        {
            var user = new User { Id = 1, Password = "old" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(1)).Returns(user);
            var handler = new ChangePasswordHandler(mockRepo.Object);
            var cmd = new ChangePasswordCommand { Id = 1, OldPassword = "old", NewPassword = "" };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }
    }
}
