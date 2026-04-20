using My_personal_budget_web_api.DTO;
using My_personal_budget_web_api.Models;

namespace My_personal_budget_web_api.Providers.Interface
{
    public interface IAccountProvider
    {
        Task<Account> CreateAccountAsync(Guid userId, CreateAccountDto createAccountDto);
        Task<Account> GetAccountByIdAsync(Guid accountId, Guid userId);
        Task<List<Account>> GetAccountsAsync(Guid userId);
    }
}
