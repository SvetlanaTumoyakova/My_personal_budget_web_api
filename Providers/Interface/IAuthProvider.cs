using My_personal_budget_web_api.DTO;
using My_personal_budget_web_api.Models;

namespace My_personal_budget_web_api.Providers.Interface
{
    public interface IAuthProvider
    {
        Task<User> RegisterUserAsync(RegisterDto dto);
        Task<User?> GetUserByLoginAsync(string login);
        Task<bool> IsUsernameTakenAsync(string userName);
        Task<bool> IsEmailTakenAsync(string email);
    }
}
