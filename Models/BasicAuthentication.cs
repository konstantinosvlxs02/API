using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DataBase;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using University.Models;

namespace WebApplication1.Models
{
    public class BasicAuthentication : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly Database _context;

        public BasicAuthentication(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, Database context)
            : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }
            try
            {
                var header=AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                if (header.Scheme != "Basic")
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Scheme"));
                }

                var bytes=  Convert.FromBase64String(header.Parameter??string.Empty);
                var credentials=Encoding.UTF8.GetString(bytes).Split(':');

                var username=credentials[0];
                var password=credentials[1];

                var user=_context.Users.FirstOrDefault(u=>u.Username==username);

                if (user == null || Password.HashPassword(password,user.Salt)!=user.Hash)
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
                }

                var claims=new[] 
                { 
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.Username)
                };

                var identity=new ClaimsIdentity(claims,Scheme.Name);
                var principal=new ClaimsPrincipal(identity);

                return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal,Scheme.Name)));
            }catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }
    }
}