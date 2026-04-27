using System.ComponentModel.DataAnnotations;

namespace My_personal_budget_web_api.DTO
{
    public class RegisterDto
    {
        [Required, MinLength(1), MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        public string LastName { get; set; }

        [Required, MinLength(1), MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string? Patronymic { get; set; }
    }
}
