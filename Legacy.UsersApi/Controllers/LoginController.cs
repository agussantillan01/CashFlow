using Legacy.UsersApi.Interfaces;
using Legacy.UsersApi.Models;
using Legacy.UsersApi.services;
using System;
using System.Web.Http;

namespace Legacy.UsersApi.Controllers
{
    public class LoginController : ApiController
    {
        private readonly IUserService _userService;

        public LoginController()
        {
            _userService = new UserService();
        }

        [HttpPost]
        [Route("api/login")]
        public IHttpActionResult Authenticate([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Credenciales inválidas.");
            }

            if (_userService.ValidateCredentials(request.Email, request.Password))
            {
                var fakeToken = Guid.NewGuid().ToString("N");
                return Ok(new { token = fakeToken, message = "Login exitoso" });
            }

            return Unauthorized();
        }
    }
}