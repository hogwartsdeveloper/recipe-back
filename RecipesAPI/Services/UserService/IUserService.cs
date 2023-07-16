using Microsoft.AspNetCore.Mvc;

namespace RecipesAPI.Services
{
    public interface IUserService
    {
        RefreshToken GenerateRefreshToken();
        string CreateToken(User user);
    }
}