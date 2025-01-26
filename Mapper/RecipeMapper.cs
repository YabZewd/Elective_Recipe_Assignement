using RecipeApi.Models;
using RecipeApp.Dtos;

namespace RecipeApp.Mapper
{
    public class RecipeMapper
    {
        public static Recipe MapToRecipe(RecipeDto recipeDto)
        {
            if (recipeDto == null)
                return null;

            return new Recipe
            {
                Title = recipeDto.Title,
                Description = recipeDto.Description,
                Instructions = recipeDto.Instructions,
                Ingredients = recipeDto.Ingredients?.Select(i => new Ingredient
                {
                    Name = i.Name,
                    Quantity = i.Quantity
                }).ToList(),
                Category = recipeDto.Category,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
