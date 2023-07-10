using Microsoft.AspNetCore.Mvc;
using recipesAPI.Services;

namespace recipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            this._userService = userService;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto required)
        {
            var user = await this._userService.Register(required);

            if (user is null)
            {
                return BadRequest("the user already has this email address");
            }

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto required)
        {
            var result = await this._userService.Login(required);

            if (result is null)
            {
                return BadRequest("User not found or wrong password");
            }

            var refreshToken = this._userService.GenerateRefreshToken();
            Response.Cookies.Append(
                "refreshToken", 
                refreshToken.Token, new CookieOptions{HttpOnly = true, Expires = refreshToken.Expired});

            return Ok(result);
        }

    }
}