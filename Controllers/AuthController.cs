using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_personal_budget_web_api.DTO.AuthDto;
using My_personal_budget_web_api.Providers.Interface;
using My_personal_budget_web_api.Service.Interface;
using System.Security.Claims;

namespace My_personal_budget_web_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthProvider _authProvider;
        private readonly IAuthService _authService;

        public AuthController(IAuthProvider authProvider, IAuthService authService)
        {
            _authProvider = authProvider;
            _authService = authService;
        }

        /// <summary>
        /// Регистрирует нового пользователя и возвращает JWT‑токен.
        /// </summary>
        /// <param name="dto">Данные для регистрации (RegisterDto).</param>
        /// <returns>IActionResult с идентификатором пользователя и JWT‑токеном при успешном выполнении.</returns>
        /// <remarks>
        /// Возможные ошибки:
        /// - 400 Bad Request: некорректные данные в запросе или ошибка бизнес‑логики (например, пользователь уже существует).
        /// - 500 Internal Server Error: внутренняя ошибка сервера.
        /// </remarks>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Хэшируем пароль
                dto.Password = _authService.HashPassword(dto.Password);

                // Регистрируем пользователя
                var user = await _authProvider.RegisterUserAsync(dto);

                // Генерируем токен
                var token = _authService.GenerateJwtToken(user);

                return CreatedAtAction(
                    nameof(Register),
                    new { id = user.Id },
                    new
                    {
                        userId = user.Id,
                        token,
                    });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при регистрации", error = ex.Message });
            }
        }

        /// <summary>
        /// Выполняет аутентификацию пользователя и возвращает JWT‑токен.
        /// </summary>
        /// <param name="loginDto">Данные для входа (LoginDto).</param>
        /// <returns>IActionResult с идентификатором пользователя и JWT‑токеном при успешной аутентификации.</returns>
        /// <remarks>
        /// Возможные ошибки:
        /// - 400 Bad Request: некорректные данные в запросе.
        /// - 401 Unauthorized: неверный логин или пароль.
        /// - 500 Internal Server Error: внутренняя ошибка сервера.
        /// </remarks>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // проверяем логин
                var user = await _authProvider.GetUserByLoginAsync(loginDto.Login);
                if (user == null)
                    return Unauthorized(new { message = "Неверный логин или пароль" });

                // проверяем пароль
                var passwordValid = _authService.VerifyPassword(loginDto.Password, user.PasswordHash);
                if (!passwordValid)
                    return Unauthorized(new { message = "Неверный логин или пароль" });

                // Генерируем токен
                var token = _authService.GenerateJwtToken(user);

                return Ok(new
                {
                    userId = user.Id,
                    token,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при входе", error = ex.Message });
            }
        }


        /// <summary>
        /// Получает данные пользователя по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор пользователя (Guid).</param>
        /// <returns>IActionResult с данными пользователя (ID, имя пользователя, имя, фамилия, email) при успешном запросе.</returns>
        /// <remarks>
        /// Возможные ошибки:
        /// - 404 Not Found: пользователь с указанным идентификатором не найден.
        /// - 500 Internal Server Error: внутренняя ошибка сервера при выполнении запроса.
        /// </remarks>
        /// 
        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            try
            {
                // Получаем ID пользователя из токена
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Пользователь не авторизован!" });
                }

                if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return BadRequest(new { message = "Недопустимый идентификатор пользователя" });
                }

                var user = await _authProvider.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "Пользователь не найден" });

                return Ok(new
                {
                    userId = user.Id,
                    userName = user.UserName,
                    email = user.Email,
                    firstName = user.Person.FirstName,
                    lastName = user.Person.LastName,
                    patronymic = user.Person?.Patronymic
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при получении пользователя", error = ex.Message });
            }
        }
    }
}
