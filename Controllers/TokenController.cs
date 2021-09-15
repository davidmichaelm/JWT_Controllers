using System;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using JWT_Controllers.Models;

namespace JWT_Controllers.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private IConfiguration _config;
        private UserManager<AppUser> _userManager;

        public TokenController(UserManager<AppUser> userManager, IConfiguration config)
        {
            _config = config;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<object> RequestToken([FromBody]UserLogin login)
        {
            IActionResult response = Unauthorized();
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(login.Username);
                if (user != null)
                {
                    var result = await _userManager.CheckPasswordAsync(user, login.Password);
                    if (result)
                    {
                        response = Ok(new { token = BuildToken(user) });
                    }
                }
            }

            return response;
        }

        private string BuildToken(AppUser user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                null, // issuer
                null, // audience
                claims,
                expires: DateTime.Now.AddDays(Int16.Parse(_config["Jwt:ValidFor"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}