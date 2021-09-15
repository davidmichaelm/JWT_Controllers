using System;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using JWT_Controllers.Models;

namespace JWT_Controllers.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class UserController : ControllerBase {
        private UserManager<AppUser> _userManager;
        public UserController(UserManager<AppUser> userManager, IConfiguration config)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<object> CreateUser([FromBody]UserLogin login)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = login.Username
                };

                var result = await _userManager.CreateAsync(user, login.Password);

                if (login.StoreId != null) {
                    var userId = await _userManager.GetUserIdAsync(user);
                    await _userManager.AddClaimAsync(user, new Claim("StoreId", login.StoreId));
                }

                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(string userId) {
            return Ok(await _userManager.FindByIdAsync(userId));
        }

        // [HttpPost("{userId}/delete")]
        // public async Task<IActionResult> DeleteUser() {

        // }
    }
}