using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Legacy.UsersApi.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}