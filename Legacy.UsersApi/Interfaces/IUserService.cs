using Legacy.UsersApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Legacy.UsersApi.Interfaces
{
    public interface IUserService
    {
        bool ValidateCredentials(string email, string password);
        UserDto GetUserById(int id);
    }
}
