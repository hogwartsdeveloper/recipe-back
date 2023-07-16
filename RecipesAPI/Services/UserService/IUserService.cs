namespace RecipesAPI.Services.UserService
{
    public interface IUserService
    {
        Task<AuthLoginResponseDto> Register(UserDto request, HttpContext context);
        Task<AuthLoginResponseDto> Login(UserDto request, HttpContext context);
        Task<AuthLoginResponseDto> UpdateToken(HttpContext context);
    }
}

