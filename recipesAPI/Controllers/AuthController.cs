using Microsoft.AspNetCore.Mvc;
using recipesAPI.Services;

namespace recipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly DataContext _context;

        public AuthController(IUserService userService, DataContext context)
        {
            this._userService = userService;
            this._context = context;
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

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> UpdateToken(UserResponseDto required)
        {
            var fUser = await this._context.Users.FirstOrDefaultAsync(u => u.Email == required.Email);
            if (fUser is null)
            {
                return BadRequest("User not found.");
            }

            var refreshToken = Request.Cookies["refreshToken"];
            if (fUser.RefreshToken != refreshToken)
            {
                return Unauthorized("Wrong token");
            } else if (fUser.TokenExpired < DateTime.Now)
            {
                return Unauthorized("Token expired");
            }
        }

    }
}