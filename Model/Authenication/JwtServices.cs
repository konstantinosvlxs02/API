using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace part2_exersice.Model.Authenication
{
    public class JwtServices
    {
        private readonly IConfiguration _configuration;

        public JwtServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateTokenForUser(User user)
        {
            var secretToken=_configuration.GetSection("Jwt:Key").Value;
            var bytes=Encoding.UTF8.GetBytes(secretToken!);

            var Claims=new []
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.Email,user.Email)
            };

            var credentials=new SigningCredentials(new SymmetricSecurityKey(bytes),SecurityAlgorithms.HmacSha256);

            var expiresMinutes=int.Parse(_configuration.GetSection("Jwt:ExpiresMinutes").Value!);  

            var token=new JwtSecurityToken(claims:Claims, expires:DateTime.Now.AddMinutes(expiresMinutes), signingCredentials:credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}