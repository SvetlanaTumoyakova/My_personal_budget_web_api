using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using My_personal_budget_web_api.DTO;
using My_personal_budget_web_api.Models;
using My_personal_budget_web_api.Providers.Interface;
using System.Security.Claims;

namespace My_personal_budget_web_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountProvider _accountProvider;

        public AccountController(IAccountProvider accountProvider)
        {
            _accountProvider = accountProvider;
        }

        /// <summary>
        /// Создаёт новый счёт для текущего пользователя на основе переданных данных.
        /// </summary>
        /// <param name="createAccountDto">Данные для создания счёта (CreateAccountDto).</param>
        /// <returns>IActionResult с данными созданного счёта (AccountDto) при успешном выполнении.</returns>
        /// <remarks>
        /// Возможные ошибки:
        /// - 400 Bad Request: некорректные данные в запросе или недопустимый идентификатор пользователя.
        /// - 401 Unauthorized: пользователь не авторизован.
        /// - 500 Internal Server Error: внутренняя ошибка сервера.
        /// </remarks>
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto createAccountDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            try
            {
                var account = await _accountProvider.CreateAccountAsync(userId, createAccountDto);

                var accountDto = new AccountDto
                {
                    Id = account.Id,
                    Name = account.Name,
                    AccountTypeId = account.AccountTypeId,
                    AccountTypeName = account.AccountType.Type,
                    Balance = account.Balance,
                    CreatedAt = account.CreatedAt
                };

                return CreatedAtAction(nameof(GetAccount), new { id = account.Id }, accountDto);
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получает информацию об аккаунте по его идентификатору (ID).
        /// </summary>
        /// <param name="id">Идентификатор аккаунта (GUID).</param>
        /// <returns>IActionResult с данными аккаунта в формате AccountDto.</returns>
        /// <remarks>
        /// Возможные коды ошибок:
        /// - 200 OK: аккаунт найден и возвращены его данные.
        /// - 401 Unauthorized: пользователь не авторизован или не имеет прав.
        /// - 404 Not Found: аккаунт с указанным ID не найден или удалён.
        /// - 500 Internal Server Error: произошла внутренняя ошибка сервера.
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Пользователь не авторизован" });
            }

            try
            {
                var account = await _accountProvider.GetAccountByIdAsync(id, userId);
                if (account == null)
                {
                    throw new InvalidOperationException("Счёт не найден или удалён");
                }
                var accountDto = new AccountDto
                {
                    Id = account.Id,
                    Name = account.Name,
                    AccountTypeId = account.AccountTypeId,
                    AccountTypeName = account.AccountType.Type,
                    Balance = account.Balance,
                    CreatedAt = account.CreatedAt
                };
                return Ok(accountDto);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Получает список аккаунтов текущего авторизованного пользователя в формате AccountDto, включая идентификатор, название, тип, баланс и дату создания.
        /// </summary>
        /// <returns>IActionResult с коллекцией AccountDto при успешном выполнении.</returns>
        /// <remarks>
        /// Возможные коды ошибок:
        /// - 401 Unauthorized: пользователь не авторизован.
        /// - 500 Internal Server Error: произошла внутренняя ошибка сервера.
        /// </remarks>

        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Пользователь не авторизован" });
            }

            try
            {
                var accounts = await _accountProvider.GetAccountsAsync(userId);
                var accountsDto = accounts.Select(account => new AccountDto
                {
                    Id = account.Id,
                    Name = account.Name,
                    AccountTypeId = account.AccountTypeId,
                    AccountTypeName = account.AccountType.Type,
                    Balance = account.Balance,
                    CreatedAt = account.CreatedAt
                }).ToList();

                return Ok(accountsDto);
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateAccountName(Guid id, [FromBody] UpdateAccountNameDto updateAccountNameDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Пользователь не авторизован" });
            }

            try
            {
                var updatedAccount = await _accountProvider.UpdateAccountNameAsync(id, userId, updateAccountNameDto.Name);

                if (updatedAccount == null)
                {
                    return NotFound(new { message = "Счёт не найден или удалён" });
                }

                return Ok(new
                {
                    message = "Имя счёта успешно обновлено",
                    accountId = id,
                    newName = updateAccountNameDto.Name,
                    updatedAt = DateTime.UtcNow
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

        /// <summary>
        /// Логически удаляет аккаунт по указанному ID. Проверяет авторизацию пользователя и право на выполнение операции. При успешном удалении возвращает подтверждение, при ошибке — соответствующий статус.
        /// </summary>
        /// <param name="id">Идентификатор аккаунта (GUID).</param>
        /// <returns>IActionResult с результатом операции.</returns>
        /// <remarks>
        /// Возможные коды ошибок:
        /// - 401 Unauthorized: пользователь не авторизован или не имеет прав.
        /// - 404 Not Found: аккаунт не найден.
        /// - 409 Conflict: аккаунт уже удалён.
        /// - 500 Internal Server Error: внутренняя ошибка сервера.
        /// </remarks>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized(new { message = "Пользователь не авторизован" });
            }

            try
            {
                var deleteResult = await _accountProvider.LogicalDeleteAccountAsync(id, userId);

                if (deleteResult == false)
                {
                    return NotFound(new { message = "Счёт не найден или уже удалён" });
                }

                return Ok(new
                {
                    message = "Счёт успешно удалён",
                    accountId = id
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Произошла внутренняя ошибка сервера" });
            }
        }

    }
}
