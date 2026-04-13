using My_personal_budget_web_api.Models;

namespace My_personal_budget_web_api.DTO
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public AccountType Type { get; set; }
        public double Balance { get; set; }
    }
}
