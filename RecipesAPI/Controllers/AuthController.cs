using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipesAPI.Services.AuthService;
using RecipesAPI.Services.UserService;

namespace RecipesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    { // Identity Server / Duende
        private readonly IUserService _userService;

        public AuthController(IUserService userService, IAuthService authService, DataContext context)
        {
            _userService = userService;
        }
        
        [HttpPost("register")]
        public async Task<ActionResult<AuthLoginResponseDto>> Register(UserDto request)
        {
            return Ok(await _userService.Register(request, HttpContext));
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthLoginResponseDto>> Login(UserDto request)
        {
            return Ok(await _userService.Login(request, HttpContext));
        }

        [HttpGet("refresh-token"), Authorize]
        public async Task<ActionResult<AuthLoginResponseDto>> UpdateToken()
        {
            return Ok(await _userService.UpdateToken(HttpContext));
        }
    }
}