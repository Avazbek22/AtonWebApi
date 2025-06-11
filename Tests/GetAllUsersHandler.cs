using Moq;

namespace GetAllUsersHandler.Tests
{
    public class User { public int Id { get; set; } public bool IsActive { get; set; } = true; }
    public interface IUserRepository { List<User> GetActiveUsers(); }

    public class GetAllActiveUsersHandler(IUserRepository repo)
    {
        public async Task<List<User>> Handle()
        {
            return await Task.FromResult(repo.GetActiveUsers());
        }
    }

    public class GetAllActiveUsersHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnActiveUsers()
        {
            var activeList = new List<User> { new User { Id = 1, IsActive = true } };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetActiveUsers()).Returns(activeList);
            var handler = new GetAllActiveUsersHandler(mockRepo.Object);

            var result = await handler.Handle();

            Assert.Equal(activeList, result);
        }
    }
}