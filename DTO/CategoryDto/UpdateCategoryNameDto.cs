using System.ComponentModel.DataAnnotations;

namespace My_personal_budget_web_api.DTO.CategoryDto
{
    public class UpdateCategoryNameDto
    {
        [Required, MinLength(1), MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
