using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using RecipeApi.Models;

namespace RecipeApp.Dtos
{
    public class RecipeDto
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Instructions are required.")]
        [MinLength(1, ErrorMessage = "At least one instruction is required.")]
        public List<string>? Instructions { get; set; }

        [Required(ErrorMessage = "Ingredients are required.")]
        [MinLength(1, ErrorMessage = "At least one ingredient is required.")]
        public List<Ingredient>? Ingredients { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        public string? Category { get; set; }
    }


}
