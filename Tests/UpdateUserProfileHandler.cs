using Moq;

namespace UpdateUserProfileHandler.Tests
{
    public class User { public int Id { get; set; } public string Name { get; set; } }

    public interface IUserRepository
    {
        User GetById(int id);
        void Update(User user);
    }

    public class UpdateUserProfileCommand { public int Id; public string Name; }
    public class UpdateUserProfileHandler(IUserRepository repo)
    {
        public async Task Handle(UpdateUserProfileCommand cmd)
        {
            var user = repo.GetById(cmd.Id);
            if (user == null) throw new KeyNotFoundException("User not found");
            if (string.IsNullOrEmpty(cmd.Name)) throw new ArgumentException("Invalid name");
            user.Name = cmd.Name;
            repo.Update(user);
            await Task.CompletedTask;
        }
    }

    public class UpdateUserProfileHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldUpdateName_WhenUserExists()
        {
            var user = new User { Id = 1, Name = "OldName" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(1)).Returns(user);
            var handler = new UpdateUserProfileHandler(mockRepo.Object);
            var cmd = new UpdateUserProfileCommand { Id = 1, Name = "NewName" };

            await handler.Handle(cmd);

            Assert.Equal("NewName", user.Name);
            mockRepo.Verify(r => r.Update(user), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenUserNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(2)).Returns((User)null!);
            var handler = new UpdateUserProfileHandler(mockRepo.Object);
            var cmd = new UpdateUserProfileCommand { Id = 2, Name = "Name" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenNameEmpty()
        {
            var user = new User { Id = 1, Name = "Name" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetById(1)).Returns(user);
            var handler = new UpdateUserProfileHandler(mockRepo.Object);
            var cmd = new UpdateUserProfileCommand { Id = 1, Name = "" };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(cmd));
            mockRepo.Verify(r => r.Update(It.IsAny<User>()), Times.Never);
        }
    }
}
