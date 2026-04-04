using Microsoft.EntityFrameworkCore;
using My_personal_budget_web_api.Context;
using My_personal_budget_web_api.DTO;
using My_personal_budget_web_api.Models;
using My_personal_budget_web_api.Providers.Interface;

namespace My_personal_budget_web_api.Providers
{
    public class AuthProvider : IAuthProvider
    {
        private readonly DataBaseContext _dataBaseContext;

        public AuthProvider(DataBaseContext dataBaseContext)
        {
            _dataBaseContext = dataBaseContext;
        }

        public async Task<User> RegisterUserAsync(RegisterDto dto)
        {

            if (await IsUsernameTakenAsync(dto.UserName))
                throw new InvalidOperationException("Пользователь с таким email уже существует");


            var person = new Person
            {
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                Patronymic = dto.Patronymic,
            };

            await _dataBaseContext.Persons.AddAsync(person);

            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                Person = person,
            };

            _dataBaseContext.Users.Add(user);

            await _dataBaseContext.SaveChangesAsync();

            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string login)
        {
            if (IsValidEmail(login))
            {
                return await _dataBaseContext.Users
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.Email == login);
            }
            else
            {
                return await _dataBaseContext.Users
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.UserName == login);
            }
        }

        public async Task<bool> IsUsernameTakenAsync(string username) // переделать
        {
            return await _dataBaseContext.Users.AnyAsync(u => u.UserName == username);
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
