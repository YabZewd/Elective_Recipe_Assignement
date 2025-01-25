using MongoDB.Driver;
using RecipeApi.Models;

namespace RecipeApi.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IMongoCollection<Recipe> _recipeCollection;

        public RecipeService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDbSettings:ConnectionString"]);
            var database = client.GetDatabase(configuration["MongoDbSettings:DatabaseName"]);
            _recipeCollection = database.GetCollection<Recipe>("Recipes");
        }

        public async Task<List<Recipe>> GetAllRecipes(string userId)
        {
            return await _recipeCollection.Find(r => r.UserId == userId).ToListAsync();
        }

        public async Task<Recipe> GetRecipeById(string id, string userId)
        {
            var recipe = await _recipeCollection.Find(r => r.Id == id && r.UserId == userId).FirstOrDefaultAsync();
            if (recipe == null)
            {
                throw new KeyNotFoundException($"Recipe with ID {id} not found or unauthorized.");
            }
            return recipe;
        }

        public async Task<Recipe> AddRecipe(Recipe recipe)
        {
            await _recipeCollection.InsertOneAsync(recipe);
            return recipe;
        }

        public async Task<Recipe> UpdateRecipe(Recipe updatedRecipe, string id, string userId)
        {
            if (updatedRecipe == null)
            {
                throw new ArgumentNullException(nameof(updatedRecipe), "Updated recipe data cannot be null.");
            }

            var filter = Builders<Recipe>.Filter.Eq(r => r.Id, id) & Builders<Recipe>.Filter.Eq(r => r.UserId, userId);
            var result = await _recipeCollection.ReplaceOneAsync(filter, updatedRecipe);

            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"Recipe with ID {id} not found or unauthorized.");
            }

            return updatedRecipe;
        }

        public async Task DeleteRecipe(string id, string userId)
        {
            var result = await _recipeCollection.DeleteOneAsync(r => r.Id == id);

            if (result.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"Recipe with ID {id} not found.");
            }
        }
    }
}
