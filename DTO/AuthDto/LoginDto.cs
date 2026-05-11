using System.ComponentModel.DataAnnotations;

namespace My_personal_budget_web_api.DTO.AuthDto
{
    public class LoginDto
    {
        [Required, MinLength(1), MaxLength(100)]
        public string Login { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
