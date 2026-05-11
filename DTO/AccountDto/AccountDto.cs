using System.ComponentModel.DataAnnotations;

namespace My_personal_budget_web_api.DTO.AccountDto
{
    public class AccountDto
    {
        public Guid Id { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid AccountTypeId { get; set; }

        [Required]
        public string AccountTypeName { get; set; } = string.Empty;

        public decimal Balance { get; set; } = 0.00m;
        public DateTime CreatedAt { get; set; }
    }
}
