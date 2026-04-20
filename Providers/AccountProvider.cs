using Microsoft.EntityFrameworkCore;
using My_personal_budget_web_api.Context;
using My_personal_budget_web_api.DTO;
using My_personal_budget_web_api.Models;
using My_personal_budget_web_api.Providers.Interface;
using System.Security.Principal;

namespace My_personal_budget_web_api.Providers
{
    public class AccountProvider : IAccountProvider
    {
        private readonly DataBaseContext _dataBaseContext;
        private const int maxAccountsPerUser = 10;

        public AccountProvider(DataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }
        public async Task<Account> CreateAccountAsync(Guid userId, CreateAccountDto createAccountDto)
        {
            // Проверка существования пользователя
            var user = await _dataBaseContext.Users
                .FirstOrDefaultAsync(u => u.Id == userId);

            // Проверка уникальности имени счёта для данного пользователя (только активные счета)
            var accountWithSameName = await _dataBaseContext.Accounts
                .FirstOrDefaultAsync(a =>
                    a.UserId == userId &&
                    a.Name == createAccountDto.Name &&
                    a.IsDeleted == false);

            if (accountWithSameName != null)
            {
                throw new InvalidOperationException("Счёт с таким именем уже существует для данного пользователя");
            }

            // Проверка на ограничение по количеству активных счетов
            var userAccountCount = await _dataBaseContext.Accounts
                .CountAsync(a => a.UserId == userId && a.IsDeleted == false);

            if (userAccountCount >= maxAccountsPerUser)
            {
                throw new InvalidOperationException($"Пользователь не может иметь более {maxAccountsPerUser} активных счетов");
            }

            var accountType = await _dataBaseContext.AccountTypes
                .FirstOrDefaultAsync(at => at.Id == createAccountDto.AccountTypeId);

            if (accountType == null)
            {
                throw new InvalidOperationException("Указанный тип счёта не существует");
            }

            var account = new Account
            {
                Id = Guid.NewGuid(),
                Name = createAccountDto.Name,
                AccountTypeId = createAccountDto.AccountTypeId,
                AccountType = accountType,
                Balance = 00.00m,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }; 

            _dataBaseContext.Accounts.Add(account);
            await _dataBaseContext.SaveChangesAsync();

            return account;
        }
        public async Task<Account> GetAccountByIdAsync(Guid accountId, Guid userId)
        {
            var account = await _dataBaseContext.Accounts
                .Include(a=>a.AccountType)
                .FirstOrDefaultAsync(a =>
                    a.Id == accountId
                    && a.UserId == userId
                    && a.IsDeleted == false);
            return account;
        }
        public async Task<List<Account>> GetAccountsAsync(Guid userId)
        {
            var accounts = await _dataBaseContext.Accounts
                .Include(a => a.AccountType)
                .Where(a => a.UserId == userId && a.IsDeleted == false) // Только активные
                .OrderBy(a => a.CreatedAt) // Сортировка по дате создания
                .ToListAsync();
            return accounts;
        }
    }
}
