using Microsoft.AspNetCore.Mvc;

namespace recipesAPI.Services
{
    public interface IUserService
    {
        Task<User?> Register(UserDto request);
        Task<string?> Login(UserDto request);
        RefreshToken GenerateRefreshToken();
    }
}