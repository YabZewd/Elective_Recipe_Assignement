using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RecipeApi.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("username")]
        public string? Username { get; set; } 

        [BsonElement("email")]
        public string? Email { get; set; } 

        [BsonElement("password")]
        public string? Password { get; set; }

        [BsonElement("favorites")]
        public List<string> Favorites { get; set; } = new List<string>();
    }
}
