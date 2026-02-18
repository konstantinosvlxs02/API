using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using part2_exersice.DataBase;
using part2_exersice.Model.API_Models;
using part2_exersice.Model;
using part2_exersice.Model.Authenication;
using Microsoft.AspNetCore.Authorization;

namespace part2_exersice.Controller
{
    [ApiController]
    [Route("auth")]
    public class UserController : ControllerBase
    {
        private readonly DataBasePart2 _context;
        private readonly JwtServices _userService;

        public UserController(DataBasePart2 context, JwtServices userService)
        {
            _context = context;
            _userService = userService;
        }

        [HttpPost("signup")]
        public IActionResult SingUp([FromBody] SingUpModel model)
        {
            var user=_context.Users.FirstOrDefault(u=>u.Username==model.Username && u.Email==model.Email);
            if(user==null)
            {
                var salt = Password.GenerateSalt();
                var newUser=new User
                {
                    Username=model.Username,
                    Email=model.Email,
                    Salt=salt,
                    Hash=Password.HashPassword(model.Password,salt)
                };
                _context.Users.Add(newUser);
                _context.SaveChanges();
                return Ok(new {message="User created successfully"});
            }
            else
            {
                return BadRequest(new {message="Username already exists"});
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
            if (user == null)
            {
                return Unauthorized(new {message="Invalid username"});
            }

            var hashedPassword = Password.HashPassword(model.Password, user.Salt);
            if (hashedPassword != user.Hash)
            {
                    return Unauthorized(new {message="Invalid password"});
            }

            var token = _userService.CreateTokenForUser(user);
            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            return Ok(new {message="User logged out successfully"});
        }
    }
}