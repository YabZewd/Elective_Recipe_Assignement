using Microsoft.AspNetCore.Mvc;
using RecipeApi.Models;
using RecipeApi.Services;
using Microsoft.AspNetCore.Authorization;
using RecipeApp.Dtos;
using RecipeApp.Mapper;
namespace RecipeApi.Controllers
{
    [ApiController]
    [Route("api/recipe")]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }


        [HttpGet]
        public async Task<ActionResult<List<Recipe>>> GetAllRecipes()
        {
            var recipes = await _recipeService.GetAllRecipes(userId);
            return Ok(recipes);
        }

        [HttpPost]
        public async Task<ActionResult<Recipe>> AddRecipe(RecipeDto newRecipe)
        {

            var userId = HttpContext.Items["UserId"]?.ToString();
            if (userId == null) return Unauthorized("Invalid token");

            var recipeModel = RecipeMapper.MapToRecipe(newRecipe);
            recipeModel.UserId = userId;
            await _recipeService.AddRecipe(recipeModel);
            return CreatedAtAction(nameof(GetRecipeById), new { id = recipeModel.Id}, recipeModel);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Recipe>> GetRecipeById(string id)
        {

            try
            {
                var recipe = await _recipeService.GetRecipeById(id);
                return Ok(recipe);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Recipe>> UpdateRecipe(string id, [FromBody] Recipe updatedRecipe)
        {
            var userId = HttpContext.Items["UserId"]?.ToString();
            if (userId == null) return Unauthorized("Invalid token");

            try
            {
                var recipe = await _recipeService.UpdateRecipe(updatedRecipe, id, userId);
                return Ok(recipe);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(string id)
        {
           var userId = HttpContext.Items["UserId"]?.ToString();
        if (userId == null) return Unauthorized("Invalid token");

        try
        {
            await _recipeService.DeleteRecipe(id, userId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        }

    }
}