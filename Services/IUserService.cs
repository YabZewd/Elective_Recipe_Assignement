using RecipeApi.Models;
using System.Threading.Tasks;

namespace RecipeApi.Services
{
    public interface IUserService
    {
        Task<User?> GetUserByEmail(string email);
        Task<User> RegisterUser(User? user);
        Task<string?> AuthenticateUser(UserLoginDto user);
        Task<bool> UpdateUser(string id, UserUpdateDto userUpdateDto); // New method
        Task<bool> DeleteUser(string id);
    }
}
