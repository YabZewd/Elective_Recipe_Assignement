using MongoDB.Driver;
using RecipeApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeApi.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IMongoCollection<User> _userCollection;

        public FavoriteService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration["MongoDbSettings:ConnectionString"]);
            var database = client.GetDatabase(configuration["MongoDbSettings:DatabaseName"]);
            _userCollection = database.GetCollection<User>("Users");
        }

        public async Task<bool> AddFavorite(string userId, string recipeId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.AddToSet(u => u.Favorites, recipeId);

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> RemoveFavorite(string userId, string recipeId)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);
            var update = Builders<User>.Update.Pull(u => u.Favorites, recipeId);

            var result = await _userCollection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<List<string>> GetFavorites(string userId)
        {
            var user = await _userCollection.Find(u => u.Id == userId).FirstOrDefaultAsync();
            return user?.Favorites ?? new List<string>();
        }
    }
}