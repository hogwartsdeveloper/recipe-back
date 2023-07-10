using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace recipesAPI.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public UserService(DataContext context, IConfiguration configuration)
        {
            this._context = context;
            this._configuration = configuration;
        }
        
        public async Task<User?> Register(UserDto request)
        {
            var fUser = await this._context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (fUser is not null)
            {
                return null;
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash
            };

            await this._context.Users.AddAsync(user);
            await this._context.SaveChangesAsync();

            return await this._context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
        }

        public async Task<string?> Login(UserDto request)
        {
            var fUser = await this._context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            if (fUser is null || !BCrypt.Net.BCrypt.Verify(request.Password, fUser.PasswordHash))
            {
                return null;
            }
            
            return this.CreateToken(fUser);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(this._configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expired = DateTime.Now.AddDays(7)
            };
        }
    }
}