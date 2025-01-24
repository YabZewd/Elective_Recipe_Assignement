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

        [HttpPut("{id}")]
        [Authorize] // Requires a valid JWT token
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto userUpdateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != id)
            {
                return Unauthorized(new { message = "You can only update your own profile" });
            }

            var result = await _userService.UpdateUser(id, userUpdateDto);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != id)
            {
                return Unauthorized(new { message = "You can only delete your own profile" });
            }

            var result = await _userService.DeleteUser(id);
            if (!result)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(new { message = "User deleted successfully" });
        }

        [HttpPost("{userId}/favorites/{recipeId}")]
        [Authorize]
        public async Task<IActionResult> AddFavorite(string userId, string recipeId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != userId)
            {
                return Unauthorized(new { message = "You can only add favorites to your own profile" });
            }

            var result = await _favoriteService.AddFavorite(userId, recipeId);
            if (!result)
            {
                return BadRequest(new { message = "Failed to add favorite" });
            }

            return Ok(new { message = "Favorite added successfully" });
        }

          [HttpDelete("{userId}/favorites/{recipeId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFavorite(string userId, string recipeId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != userId)
            {
                return Unauthorized(new { message = "You can only remove favorites from your own profile" });
            }

            var result = await _favoriteService.RemoveFavorite(userId, recipeId);
            if (!result)
            {
                return BadRequest(new { message = "Failed to remove favorite" });
            }

            return Ok(new { message = "Favorite removed successfully" });
        }

        [HttpGet("{userId}/favorites")]
        [Authorize]
        public async Task<IActionResult> GetFavorites(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != userId)
            {
                return Unauthorized(new { message = "You can only view your own favorites" });
            }

            var favorites = await _favoriteService.GetFavorites(userId);
            return Ok(favorites);
        }
            
    }
}