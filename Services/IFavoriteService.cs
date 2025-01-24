using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApi.Services
{
    public interface IFavoriteService
    {
        Task<bool> AddFavorite(string userId, string recipeId);
        Task<bool> RemoveFavorite(string userId, string recipeId);
        Task<List<string>> GetFavorites(string userId);
    }
}