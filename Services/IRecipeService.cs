using RecipeApi.Models;
using System.Threading.Tasks;

namespace RecipeApi.Services
{
    public interface IRecipeService
    {
        Task<List<Recipe>> GetAllRecipes(string userId);
        Task<Recipe> GetRecipeById(string id);
        Task<Recipe> AddRecipe(Recipe recipe);
        Task<Recipe> UpdateRecipe(Recipe recipe, string id, string userId);
        Task DeleteRecipe(string id, string userId);
    }
}
