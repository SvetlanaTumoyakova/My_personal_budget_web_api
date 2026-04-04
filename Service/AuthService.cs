using My_personal_budget_web_api.Models;
using My_personal_budget_web_api.Service.Interface;

namespace My_personal_budget_web_api.Service
{
    public class AuthService : IAuthService
    {
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher _passwordHasher;
        public AuthService(IJwtService jwtService, IPasswordHasher passwordHasher)
        {
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }
        public string GenerateJwtToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            return _jwtService.GenerateToken(user);
        }

        public string HashPassword(string password)
        {
            if (password is null) throw new ArgumentNullException(nameof(password));
            return _passwordHasher.HashPassword(password);
        }
        public bool VerifyPassword(string password, string hash)
        {
            return _passwordHasher.VerifyPassword(password, hash);
        }
    }
}
