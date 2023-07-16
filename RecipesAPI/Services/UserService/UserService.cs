using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RecipesAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;

        public UserService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        
        public string CreateToken(User user)
        {
            var expired = DateTime.Now.AddMinutes(15);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Expired, $"{expired.ToLongDateString()} {expired.ToLongTimeString()}")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(this._configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: expired,
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