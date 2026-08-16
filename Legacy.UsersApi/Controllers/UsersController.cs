using Legacy.UsersApi.Interfaces;
using Legacy.UsersApi.Models;
using Legacy.UsersApi.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;


namespace Legacy.UsersApi.Controllers
{
    public class UsersController : ApiController
    {

        private readonly IUserService _userService;

        public UsersController()
        {
            _userService = new UserService();
        }

        [HttpGet]
        [Route("api/users/{id}")]
        public IHttpActionResult GetUserById(int id)
        {
            var userDto = _userService.GetUserById(id);

            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }
    }
}