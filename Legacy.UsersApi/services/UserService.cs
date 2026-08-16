using Legacy.UsersApi.Interfaces;
using Legacy.UsersApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Legacy.UsersApi.services
{
    public class UserService : IUserService
    {
        private static readonly List<User> _users = new List<User>
        {
            new User { Id = 1, Email = "admin@blackwallet.com", Password = "1234", Name = "Administrador" },
            new User { Id = 2, Email = "operador@blackwallet.com", Password = "1234", Name = "Operador" }
        };

        public bool ValidateCredentials(string email, string password)
        {
            return _users.Any(u =>
                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                && u.Password == password);
        }

        public UserDto GetUserById(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }
    }
}