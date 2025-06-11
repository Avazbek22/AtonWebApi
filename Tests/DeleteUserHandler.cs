using Moq;

namespace DeleteUserHandler.Tests
{
    public class User { public int Id { get; set; } }

    public interface IUserRepository
    {
        User GetById(int id);
        void SoftDelete(int id);
    }

    public class DeleteUserCommand { public int Id; }
    public class DeleteUserHandler(IUserRepository repo)
    {
        public async Task Handle(DeleteUserCommand cmd)
        {
            var user = repo.GetById(cmd.Id);
            if (user == null) throw new KeyNotFoundException("User not found");
            repo.SoftDelete(cmd.Id);
            await Task.CompletedTask;
        }
    }

    public class DeleteUserHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldSoftDelete_WhenUserExists()
        {
            var user = new User { Id = 5 };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(5)).Returns(user);
            var handler = new DeleteUserHandler(mockRepo.Object);
            var cmd = new DeleteUserCommand { Id = 5 };

            await handler.Handle(cmd);

            mockRepo.Verify(r => r.SoftDelete(5), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(6)).Returns((User)null!);
            var handler = new DeleteUserHandler(mockRepo.Object);
            var cmd = new DeleteUserCommand { Id = 6 };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(cmd));
        }
    }
}