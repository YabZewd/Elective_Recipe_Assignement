using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeApi.Models;
using RecipeApi.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RecipeApi.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IFavoriteService _favoriteService;

        public UserController(IUserService userService, IFavoriteService favoriteService)
        {
            _userService = userService;
            _favoriteService = favoriteService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto user)
        {
            User? userInDb = await _userService.GetUserByEmail(user?.Email);
            if (userInDb != null)
            {
                return BadRequest(new { message = "Email is already in use" });
            }

            User createdUser = new User { Username = user.UserName, Email = user.Email, Password = user.Password };
            await _userService.RegisterUser(createdUser);
            return Ok(new
            {
                message = "User registered successfully",
                user = new UserReturnDto
                {
                    UserName = user.UserName,
                    Email = user.Email
                }
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto user)
        {
            if (string.IsNullOrEmpty(user.Email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            var token = await _userService.AuthenticateUser(user);
            if (token == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            return Ok(new { token });
        }
 }
}