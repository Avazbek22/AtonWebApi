using Moq;

namespace GetUserByLoginHandler.Tests
{
    public class User { public int Id { get; set; } public string Login { get; set; } }
    public interface IUserRepository { User GetByLogin(string login); }

    public class GetUserByLoginQuery { public string Login; }
    public class GetUserByLoginHandler(IUserRepository repo)
    {
        public async Task<User> Handle(GetUserByLoginQuery query)
        {
            var user = repo.GetByLogin(query.Login);
            if (user == null) throw new KeyNotFoundException("User not found");
            return await Task.FromResult(user);
        }
    }

    public class GetUserByLoginHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnUser_WhenFound()
        {
            var user = new User { Id = 1, Login = "user1" };
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByLogin("user1")).Returns(user);
            var handler = new GetUserByLoginHandler(mockRepo.Object);
            var query = new GetUserByLoginQuery { Login = "user1" };

            var result = await handler.Handle(query);

            Assert.Equal(user, result);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenNotFound()
        {
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByLogin("user2")).Returns((User)null!);
            var handler = new GetUserByLoginHandler(mockRepo.Object);
            var query = new GetUserByLoginQuery { Login = "user2" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => handler.Handle(query));
        }
    }
}