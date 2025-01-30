using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace RecipeApi.Models
{
    public class Recipe
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public string? Title { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("instructions")]
        public List<string>? Instructions { get; set; }

        [BsonElement("ingredients")]
        public List<Ingredient>? Ingredients { get; set; }

        [BsonElement("category")]
        public string? Category { get; set; }

        [BsonElement("createdBy")]
        public string? UserId { get; set; }

        [BsonElement("createdAt")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Ingredient
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("quantity")]
        public string Quantity { get; set; }
    }
}