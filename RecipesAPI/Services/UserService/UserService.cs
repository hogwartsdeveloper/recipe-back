using System.Net;
using System.Security.Claims;
using RecipesAPI.Services.AuthService;

namespace RecipesAPI.Services.UserService
{
    public class UserService: IUserService
    {
        private readonly DataContext _context;
        private readonly IAuthService _authService;

        public UserService(DataContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        
        public async Task<AuthLoginResponseDto> Register(UserDto request, HttpContext context)
        {
            var fUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (fUser is not null)
            {
                throw new CustomException((int)HttpStatusCode.Conflict, "This email is already registered.");
            }
            
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash
            };
            var token = _authService.CreateToken(user);
            await _context.Users.AddAsync(user);
            SetRefreshToken(context, _authService.GenerateRefreshToken(), user);
            await _context.SaveChangesAsync();
            return new AuthLoginResponseDto { Token = token };
        }

        public async Task<AuthLoginResponseDto> Login(UserDto request, HttpContext context)
        {
            var fUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (fUser is null)
            {
                throw new CustomException((int)HttpStatusCode.BadRequest, "User with this email was not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, fUser.PasswordHash))
            {
                throw new CustomException((int)HttpStatusCode.Unauthorized, "Wrong password");
            }
            
            SetRefreshToken(context, _authService.GenerateRefreshToken(), fUser);
            await _context.SaveChangesAsync();

            return new AuthLoginResponseDto { Token = _authService.CreateToken(fUser) };
        }

        public async Task<AuthLoginResponseDto> UpdateToken(HttpContext context)
        {
            var fUser = await this._context.Users.FirstOrDefaultAsync(
                u => u.Email == context.User.FindFirstValue(ClaimTypes.Email));
            if (fUser is null)
            {
                throw new CustomException((int)HttpStatusCode.BadRequest, "User not found.");
            }

            var refreshToken = context.Request.Cookies["refreshToken"];
            if (fUser.RefreshToken != refreshToken)
            {
                throw new CustomException((int)HttpStatusCode.Unauthorized, "Wrong token");
            } else if (fUser.TokenExpired < DateTime.Now)
            {
                throw new CustomException((int)HttpStatusCode.Unauthorized, "Token expired");
            }
            
            var token = this._authService.CreateToken(fUser);

            return new AuthLoginResponseDto { Token = token };
        }

        private void SetRefreshToken(HttpContext context, RefreshToken refreshToken, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expired,
                Path = "/"
            };
            
            context.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
            user.RefreshToken = refreshToken.Token;
            user.TokenCreated = refreshToken.Created;
            user.TokenExpired = refreshToken.Expired;
        }
    }
}

