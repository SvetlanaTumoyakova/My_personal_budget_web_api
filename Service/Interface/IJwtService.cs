using My_personal_budget_web_api.Models;

namespace My_personal_budget_web_api.Service.Interface
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
