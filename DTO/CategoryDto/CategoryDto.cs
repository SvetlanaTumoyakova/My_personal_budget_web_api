using My_personal_budget_web_api.Models;
using System.ComponentModel.DataAnnotations;

namespace My_personal_budget_web_api.DTO.CategoryDto
{
    public class CategoryDto
    {
        public Guid Id { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public bool IsDefault { get; set; }

        [Required]
        public Guid TransactionTypeId { get; set; }

        [Required]
        public string TransactionType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
