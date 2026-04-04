using System.ComponentModel.DataAnnotations;

namespace My_personal_budget_web_api.DTO
{
    public class LoginDto
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
