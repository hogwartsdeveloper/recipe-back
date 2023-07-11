using Microsoft.AspNetCore.Mvc;

namespace recipesAPI.Services
{
    public interface IUserService
    {
        RefreshToken GenerateRefreshToken();
        string CreateToken(User user);
    }
}