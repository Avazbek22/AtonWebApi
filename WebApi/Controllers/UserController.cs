using Application.Commands;
using Application.DTOs;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Вспомогательный метод: получаем текущего пользователя из заголовка X-Current-User
        /// </summary>
        private string? GetCurrentUserLogin()
        {
            if (Request.Headers.TryGetValue("X-Current-User", out var vals))
            {
                return vals.ToString();
            }
            return null;
        }

        /// <summary>
        /// 1) Создание пользователя (только для админа)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var currentUser = GetCurrentUserLogin();
            if (string.IsNullOrWhiteSpace(currentUser))
                return BadRequest("Требуется заголовок X-Current-User");

            var result = await mediator.Send(new CreateUserCommand(dto, currentUser));
            if (!result)
                return BadRequest("Не удалось создать пользователя (возможно, логин занят или нет прав).");

            return Ok("Пользователь успешно создан.");
        }

        /// <summary>
        /// 2) Изменение имени, пола или даты рождения
        /// </summary>
        [HttpPut("profile/{login}")]
        public async Task<IActionResult> UpdateProfile(string login, [FromQuery] string? newName, [FromQuery] int? newGender,
            [FromQuery] DateTime? newBirthday)
        {
            var currentUser = GetCurrentUserLogin();
            if (string.IsNullOrWhiteSpace(currentUser))
                return BadRequest("Требуется заголовок X-Current-User");

            var cmd = new UpdateUserProfileCommand(login, newName, newGender, newBirthday, currentUser);
            var result = await mediator.Send(cmd);
            if (!result)
                return BadRequest("Не удалось обновить профиль (нет прав или пользователь не найден).");

            return Ok("Профиль успешно обновлён.");
        }

        /// <summary>
        /// 3) Изменение пароля
        /// </summary>
        [HttpPut("password/{login}")]
        public async Task<IActionResult> ChangePassword(string login, [FromQuery] string newPassword)
        {
            var currentUser = GetCurrentUserLogin();
            if (string.IsNullOrWhiteSpace(currentUser))
                return BadRequest("Требуется заголовок X-Current-User");

            var cmd = new ChangePasswordCommand(login, newPassword, currentUser);
            var result = await mediator.Send(cmd);
            if (!result)
                return BadRequest("Не удалось изменить пароль (нет прав или пользователь не найден).");

            return Ok("Пароль успешно изменён.");
        }

        /// <summary>
        /// 4) Изменение логина
        /// </summary>
        [HttpPut("login/{login}")]
        public async Task<IActionResult> ChangeLogin(string login, [FromQuery] string newLogin)
        {
            var currentUser = GetCurrentUserLogin();
            if (string.IsNullOrWhiteSpace(currentUser))
                return BadRequest("Требуется заголовок X-Current-User");

            var cmd = new ChangeLoginCommand(login, newLogin, currentUser);
            var result = await mediator.Send(cmd);
            if (!result)
                return BadRequest("Не удалось изменить логин (нет прав или логин занят).");

            return Ok("Логин успешно изменён.");
        }

        /// <summary>
        /// 5) Запрос списка всех активных пользователей (только админы)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllActive()
        {
            var currentUser = GetCurrentUserLogin();
            if (string.IsNullOrWhiteSpace(currentUser))
                return BadRequest("Требуется заголовок X-Current-User");

            var query = new GetAllActiveUsersQuery(currentUser);
            var users = await mediator.Send(query);
            return Ok(users);
        }

        /// <summary>
        /// 6) Запрос пользователя по логину (только админы)
        /// </summary>
        [HttpGet("{login}")]
        public async Task<IActionResult> GetByLogin(string login)
        {
            var currentUser = GetCurrentUserLogin();
            if (string.IsNullOrWhiteSpace(currentUser))
                return BadRequest("Требуется заголовок X-Current-User");

            var query = new GetUserByLoginQuery(login, currentUser);
            var user = await mediator.Send(query);
            if (user is null)
                return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// 7) Запрос пользователя по логину и паролю (сам только для себя, если активен)
        /// </summary>
        [HttpGet("authenticate")]
        public async Task<IActionResult> Authenticate([FromQuery] string login, [FromQuery] string password)
        {
            var query = new AuthenticateUserQuery(login, password);
            var user = await mediator.Send(query);
            if (user is null)
                return BadRequest("Неверный логин/пароль или пользователь неактивен.");
            return Ok(user);
        }

        /// <summary>
        /// 8) Запрос всех пользователей старше возраста (только админы)
        /// </summary>
        [HttpGet("older-than/{age}")]
        public async Task<IActionResult> GetOlderThan(int age)
        {
            var currentUser = GetCurrentUserLogin();
            if (string.IsNullOrWhiteSpace(currentUser))
                return BadRequest("Требуется заголовок X-Current-User");

            var query = new GetUsersOlderThanQuery(age, currentUser);
            var users = await mediator.Send(query);
            return Ok(users);
        }

        /// <summary>
        /// 9) Удаление пользователя по логину (полное или мягкое) (только админы)
        /// </summary>
        [HttpDelete("{login}")]
        public async Task<IActionResult> DeleteUser(string login, [FromQuery] bool hard = false)
        {
            var currentUser = GetCurrentUserLogin();
            if (string.IsNullOrWhiteSpace(currentUser))
                return BadRequest("Требуется заголовок X-Current-User");

            var cmd = new DeleteUserCommand(login, hard, currentUser);
            var result = await mediator.Send(cmd);
            if (!result)
                return BadRequest("Не удалось удалить пользователя (нет прав или не найден).");

            return Ok(hard ? "Пользователь полностью удалён." : "Пользователь мягко удалён.");
        }

        /// <summary>
        /// 10) Восстановление пользователя (только админы)
        /// </summary>
        [HttpPut("restore/{login}")]
        public async Task<IActionResult> RestoreUser(string login)
        {
            var currentUser = GetCurrentUserLogin();
            if (string.IsNullOrWhiteSpace(currentUser))
                return BadRequest("Требуется заголовок X-Current-User");

            var cmd = new RestoreUserCommand(login, currentUser);
            var result = await mediator.Send(cmd);
            if (!result)
                return BadRequest("Не удалось восстановить пользователя (нет прав или пользователь не удалён).");

            return Ok("Пользователь успешно восстановлен.");
        }
    }