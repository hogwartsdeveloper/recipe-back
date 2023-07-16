namespace RecipesAPI.Services.AuthService
{
    public interface IAuthService
    {
        RefreshToken GenerateRefreshToken();
        string CreateToken(User user);
    }
}