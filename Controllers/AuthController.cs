using Microsoft.AspNetCore.Mvc;
using My_personal_budget_web_api.DTO;
using My_personal_budget_web_api.Providers.Interface;
using My_personal_budget_web_api.Service.Interface;

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // проверяем логин
                var user = await _authProvider.GetUserByLoginAsync(dto.Login);
                if (user == null)
                    return Unauthorized(new { message = "Неверный логин или пароль" });

                // проверяем пароль
                var passwordValid = _authService.VerifyPassword(dto.Password, user.PasswordHash);
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
    }
}
