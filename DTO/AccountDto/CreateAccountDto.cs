using System.ComponentModel.DataAnnotations;

namespace My_personal_budget_web_api.DTO.AccountDto
{
    public class CreateAccountDto
    {
        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid AccountTypeId { get; set; }
    }
}
