using My_personal_budget_web_api.Models;
using System.ComponentModel.DataAnnotations;

namespace My_personal_budget_web_api.DTO
{
    public class CreateAccountDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid AccountTypeId { get; set; }
    }
}
