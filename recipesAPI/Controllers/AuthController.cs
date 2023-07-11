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
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var fUser = await this._context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (fUser is not null)
            {
                return Conflict("This email is already registered.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            await this._context.Users.AddAsync(new User
            {
                Email = request.Email,
                PasswordHash = passwordHash
            });
            await this._context.SaveChangesAsync();

            return Ok(await this._context.Users.FirstOrDefaultAsync(u => u.Email == request.Email));
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var fUser = await this._context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (fUser is null)
            {
                return BadRequest("User with this email was not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, fUser.PasswordHash))
            {
                return Unauthorized("Wrong password");
            }
            
            this.SetRefreshToken(this._userService.GenerateRefreshToken(), fUser);
            await this._context.SaveChangesAsync();
            
            return Ok(this._userService.CreateToken(fUser));

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
            
            this.SetRefreshToken(this._userService.GenerateRefreshToken(), fUser);
            await this._context.SaveChangesAsync();

            return Ok(this._userService.CreateToken(fUser));
        }

        private void SetRefreshToken(RefreshToken refreshToken, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expired
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            user.RefreshToken = refreshToken.Token;
            user.TokenCreated = refreshToken.Created;
            user.TokenExpired = refreshToken.Expired;
        }

    }
}