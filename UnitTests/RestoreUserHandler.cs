using Moq;

namespace RestoreUserHandler.Tests
{
    public class User { public int Id { get; set; } }

    public interface IUserRepository
    {
        User GetById(int id);
        void Restore(int id);
    }

    public class RestoreUserCommand { public int Id; }
    public class RestoreUserHandler(IUserRepository repo)
    {
        public async Task Handle(RestoreUserCommand cmd)
        {
            var user = repo.GetById(cmd.Id);
            if (user == null) throw new KeyNotFoundException("User not found");
            repo.Restore(cmd.Id);
            await Task.CompletedTask;
        }
    }

    public class RestoreUserHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldRestore_WhenUserExists()
        {
            var user = new User { Id = 7 };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(7)).Returns(user);
            var handler = new RestoreUserHandler(mockRepo.Object);
            var cmd = new RestoreUserCommand { Id = 7 };

            await handler.Handle(cmd);

            mockRepo.Verify(r => r.Restore(7), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(8)).Returns((User)null!);
            var handler = new RestoreUserHandler(mockRepo.Object);
            var cmd = new RestoreUserCommand { Id = 8 };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(cmd));
        }
    }
}