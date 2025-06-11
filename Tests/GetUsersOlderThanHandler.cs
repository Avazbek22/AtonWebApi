using Moq;

namespace GetUsersOlderThanHandler.Tests
{
    public class User { public int Id { get; set; } public int Age { get; set; } }
    public interface IUserRepository { List<User> GetUsersOlderThan(int age); }

    public class GetUsersOlderThanQuery { public int Age; }
    public class GetUsersOlderThanHandler(IUserRepository repo)
    {
        public async Task<List<User>> Handle(GetUsersOlderThanQuery query)
        {
            if (query.Age < 0) throw new ArgumentException("Invalid age");
            return await Task.FromResult(repo.GetUsersOlderThan(query.Age));
        }
    }

    public class GetUsersOlderThanHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnUsers_WhenAgeValid()
        {
            var list = new List<User> { new User { Id = 3, Age = 40 }, new User { Id = 4, Age = 50 } };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetUsersOlderThan(30)).Returns(list);
            var handler = new GetUsersOlderThanHandler(mockRepo.Object);
            var query = new GetUsersOlderThanQuery { Age = 30 };

            var result = await handler.Handle(query);

            Assert.Equal(list, result);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenAgeInvalid()
        {
            var mockRepo = new Mock<IUserRepository>();
            var handler = new GetUsersOlderThanHandler(mockRepo.Object);
            var query = new GetUsersOlderThanQuery { Age = -1 };

            await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query));
        }
    }
}