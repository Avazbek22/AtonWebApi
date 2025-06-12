using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using WebApi;

namespace IntegrationTests
{
    public class UserApiIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private void AddCurrentUser(string login)
        {
            _client.DefaultRequestHeaders.Remove("X-Current-User");
            if (!string.IsNullOrEmpty(login))
                _client.DefaultRequestHeaders.Add("X-Current-User", login);
        }

        [Fact]
        public async Task CreateUser_AsAdmin_ReturnsOk()
        {
            AddCurrentUser("Admin");
            var dto = new
            {
                login = "testuser",
                password = "Password123@",
                name = "Test User",
                gender = 1,
                birthday = DateTime.UtcNow,
                admin = false
            };

            var response = await _client.PostAsJsonAsync("api/users", dto);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_WithoutHeader_ReturnsBadRequest()
        {
            AddCurrentUser(null!);
            var dto = new
            {
                login = "noheader", password = "p", name = "N", gender = 0, birthday = (DateTime?)null, admin = false
            };
            var response = await _client.PostAsJsonAsync("api/users", dto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [InlineData("", "p", "Name", 1)] // empty login
        [InlineData("valid", "", "Name", 1)] // empty password
        [InlineData("valid", "pass", "", 1)] // empty name
        [InlineData("valid", "pass123", "Name", 5)] // invalid gender
        public async Task CreateUser_InvalidDto_ReturnsBadRequest(string login, string password, string name, int gender)
        {
            AddCurrentUser("Admin");
            CreateUserDto dto = new()
            {
                Admin = false, Birthday = DateTime.Now, Login = login, Password = password, Name = name, Gender = gender
            };
            var response = await _client.PostAsJsonAsync("api/users", dto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetAllActive_AsNonAdmin_ReturnsBadRequest()
        {
            AddCurrentUser("user1");
            var response = await _client.GetAsync("api/users");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetAllActive_AsAdmin_ReturnsOk()
        {
            AddCurrentUser("Admin");
            var response = await _client.GetAsync("api/users");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetByLogin_NotExists_ReturnsNotFound()
        {
            AddCurrentUser("Admin");
            var response = await _client.GetAsync("api/users/nonexistent");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Authenticate_InvalidCredentials_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("api/users/authenticate?login=foo&password=bar");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GetOlderThan_NegativeAge_ReturnsBadRequest()
        {
            AddCurrentUser("Admin");
            var response = await _client.GetAsync("api/users/older-than/-1");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_NonExistentUser_ReturnsBadRequest()
        {
            AddCurrentUser("Admin");
            var response = await _client.DeleteAsync("api/users/unknown?hard=true");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteSelfAsAdmin_BadRequest()
        {
            AddCurrentUser("Admin");
            var response = await _client.DeleteAsync("api/users/Admin?hard=true");
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteUser_SoftThenAuthenticate_ReturnsBadRequest()
        {
            AddCurrentUser("Admin");
            // ensure user exists
            await _client.PostAsJsonAsync("api/users",
                new
                {
                    login = "temp", password = "Password123@", name = "Temp", gender = 0, birthday = (DateTime?)null, admin = false
                });
            // soft delete
            var deleteResp = await _client.DeleteAsync("api/users/temp");
            Assert.Equal(HttpStatusCode.OK, deleteResp.StatusCode);
            // authenticate
            var authResp = await _client.GetAsync("api/users/authenticate?login=temp&password=p");
            Assert.Equal(HttpStatusCode.Unauthorized, authResp.StatusCode);
        }
        
        [Fact]
        public async Task UpdateProfile_AsAnotherUser_ReturnsBadRequest()
        {
            AddCurrentUser("user1");
            var response = await _client.PutAsync("api/users/profile/testuser?newName=Hacker", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProfile_AsSelf_ChangesName()
        {
            AddCurrentUser("testuser");

            var response = await _client.PutAsync("api/users/profile/testuser?newName=UpdatedName", null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_AsAnotherUser_ReturnsBadRequest()
        {
            AddCurrentUser("user1");
            var response = await _client.PutAsync("api/users/password/testuser?newPassword=NewPass", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangePassword_ThenAuthenticate_Success()
        {
            // 1. Создаём пользователя Admin (если нужно), чтобы иметь права
            AddCurrentUser("Admin");

            // 2. Создаём пользователя testuser с исходным паролем
            var createDto = new
            {
                login = "testuser",
                password = "Password123@",
                name = "Test User",
                gender = 1,
                birthday = DateTime.UtcNow,
                admin = false
            };
            var createResp = await _client.PostAsJsonAsync("api/users", createDto);
            Assert.Equal(HttpStatusCode.OK, createResp.StatusCode);

            // 3. Меняем пароль от имени самого testuser (или Admin — зависит от логики разрешений)
            AddCurrentUser("testuser");
            var changePassResp = await _client.PutAsync("api/users/password/testuser?newPassword=Password123@", null);
            Assert.Equal(HttpStatusCode.OK, changePassResp.StatusCode);

            // 4. Пробуем аутентифицироваться с новым паролем
            var authResp = await _client.GetAsync("api/users/authenticate?login=testuser&password=Password123@");
            Assert.Equal(HttpStatusCode.OK, authResp.StatusCode);
        }


        [Fact]
        public async Task ChangeLogin_ToExistingLogin_ReturnsBadRequest()
        {
            AddCurrentUser("testuser");
            await _client.PostAsJsonAsync("api/users", new
            {
                login = "existing", password = "Password123@", name = "E", gender = 0, birthday = (DateTime?)null, admin = false
            });

            var response = await _client.PutAsync("api/users/login/testuser?newLogin=existing", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ChangeLogin_ThenAuthenticateWithNewLogin_Success()
        {
            AddCurrentUser("testuser");
            await _client.PutAsync("api/users/login/testuser?newLogin=newloginuser", null);

            var authResp = await _client.GetAsync("api/users/authenticate?login=newloginuser&password=Password123@");
            Assert.Equal(HttpStatusCode.OK, authResp.StatusCode);
        }

        [Fact]
        public async Task Restore_NonExistentUser_ReturnsBadRequest()
        {
            AddCurrentUser("Admin");
            var response = await _client.PutAsync("api/users/restore/ghost", null);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Restore_SoftDeletedUser_Success()
        {
            AddCurrentUser("Admin");

            await _client.PostAsJsonAsync("api/users", new
            {
                login = "restorable", password = "Password!123", name = "Res", gender = 0, birthday = (DateTime?)null, admin = false
            });

            await _client.DeleteAsync("api/users/restorable"); // soft delete

            var restoreResp = await _client.PutAsync("api/users/restore/restorable", null);
            Assert.Equal(HttpStatusCode.OK, restoreResp.StatusCode);
        }

        [Fact]
        public async Task GetOlderThan_ValidAge_AsAdmin_ReturnsOk()
        {
            AddCurrentUser("Admin");
            var response = await _client.GetAsync("api/users/older-than/18");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}