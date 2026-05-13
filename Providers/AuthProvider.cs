using Microsoft.EntityFrameworkCore;
using My_personal_budget_web_api.Context;
using My_personal_budget_web_api.DTO.AuthDto;
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
                throw new InvalidOperationException("Пользователь с таким именем уже существует");

            if (await IsEmailTakenAsync(dto.Email))
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

        public async Task<User?> GetUserByLoginAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
            {
                return null;
            }

            if (IsValidEmail(login))
            {
                return await _dataBaseContext.Users
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == login.ToLower());
            }
            else
            {
                return await _dataBaseContext.Users
                    .Include(u => u.Person)
                    .FirstOrDefaultAsync(u => u.UserName.ToLower() == login.ToLower());
            }
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _dataBaseContext.Users
                .Include(u => u.Person)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Проверяет, занят ли указанное имя пользователя в системе.
        /// </summary>
        /// <param name="userName">Имя пользователя для проверки.</param>
        /// <returns><c>true</c>, если имя пользователя уже зарегистрированно; иначе <c>false</c>.</returns>
        public async Task<bool> IsUsernameTakenAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            return await _dataBaseContext.Users.AnyAsync(u =>
                u.UserName.ToLower() == userName.ToLower());
        }

        /// <summary>
        /// Проверяет, занят ли указанный email в системе.
        /// </summary>
        /// <param name="email">Email пользователя для проверки.</param>
        /// <returns><c>true</c>, если email уже зарегистрирован; иначе <c>false</c>.</returns>
        public async Task<bool> IsEmailTakenAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return await _dataBaseContext.Users.AnyAsync(u =>
                u.Email.ToLower() == email.ToLower());
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
