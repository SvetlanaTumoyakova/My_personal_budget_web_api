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
        /// Создание нового счёта для текущего пользователя
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountDto createAccountDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        ///Получение одного аккаунта по его id
        /// </summary>
        /// 
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
        ///Получение списка всех активных аккаунтов пользователя
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                return Unauthorized(new { message = "Пользователь не авторизован" });

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
    }
}
