using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using recipesAPI.Services;

namespace recipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    { // Identity Server / Duende
        private readonly IUserService _userService;
        private readonly DataContext _context;

        public AuthController(IUserService userService, DataContext context)
        {
            this._userService = userService;
            this._context = context;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<AuthLoginResponseDto>> Register(UserDto request)
        {
            var fUser = await this._context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (fUser is not null)
            {
                return Conflict("This email is already registered.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash
            };

            var token = this._userService.CreateToken(user);
            this.SetRefreshToken(this._userService.GenerateRefreshToken(), user);
            await this._context.Users.AddAsync(user);
            await this._context.SaveChangesAsync();
            
            return Ok(new AuthLoginResponseDto{Token = token});
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthLoginResponseDto>> Login(UserDto request)
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

            var token = this._userService.CreateToken(fUser);
            this.SetRefreshToken(this._userService.GenerateRefreshToken(), fUser);
            await this._context.SaveChangesAsync();
            
            return Ok(new AuthLoginResponseDto{Token = token});

        }

        [HttpPost("refresh-token"), Authorize]
        public async Task<ActionResult<string>> UpdateToken()
        {
            var fUser = await this._context.Users.FirstOrDefaultAsync(u => u.Email == User.FindFirstValue(ClaimTypes.Email));
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