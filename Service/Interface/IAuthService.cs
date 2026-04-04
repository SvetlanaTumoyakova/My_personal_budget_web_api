using My_personal_budget_web_api.Models;

namespace My_personal_budget_web_api.Service.Interface
{
    public interface IAuthService
    {
        string HashPassword(string password);
        string GenerateJwtToken(User user);
        bool VerifyPassword(string password, string hash);
    }
}
