using CookingRecipePortal.DTO_s;
using CookingRecipePortal.Infrastructure;
using CookingRecipePortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CookingRecipePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors]
    public class RecipeController : ControllerBase
    {

        private readonly IRecipeRepository _recipeRepository;

        public RecipeController(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AddRecipe(RecipeDto recipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    
                var newRecipe = new Recipe
                {
                    Title = recipe.Title,
                    Description = recipe.Description,
                    IsVegetarian = recipe.IsVegetarian,
                    Ingredients = recipe.Ingredients,
                    Instructions = recipe.Instructions,
                    ImageUrl = recipe.ImageUrl, // Handle image URL if provided
                    Region = recipe.Region,
                    CreatedAt = DateTime.Now,
                    UserId = userId
                };

                await _recipeRepository.AddRecipeAsync(newRecipe);
                

                return Ok(recipe);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        // GET: api/recipe
        [AllowAnonymous] // Public
        [HttpGet]
        public async Task<IActionResult> GetAllRecipes()
        {
            try
            {
                var recipes = await _recipeRepository.GetAllRecipesAsync();
                var recipeDtos = recipes.Select(recipe => new RecipeDetailsDto
                {
                    RecipeId = recipe.RecipeId,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients,
                    Instructions = recipe.Instructions,
                    ImageUrl = recipe.ImageUrl,
                    IsVegetarian = recipe.IsVegetarian,
                    Region = recipe.Region,
                    CreatedAt = recipe.CreatedAt,
                    UserId = recipe.UserId,
                    User = new UserDto
                    {
                        UserId = recipe.User.UserId,
                        Username = recipe.User.Name,
                        Email = recipe.User.Email
                    },
                    Feedbacks = recipe.Feedbacks.Select(f => new FeedbackDetailsDto
                    {
                        FeedbackId = f.FeedbackId,
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CreatedAt = f.CreatedAt
                    }).ToList()
                }).ToList();
                
                return Ok(recipeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/recipe/filter?isVegetarian=true&region=Karnataka
        [AllowAnonymous]
        [HttpGet("filter")]
        public async Task<IActionResult> FilterRecipes(string? name, string? category, string? region)
        {
            try
            {
                var recipes = await _recipeRepository.FilterRecipesAsync(name, category, region);
                var recipeDtos = recipes.Select(recipe => new RecipeDetailsDto
                {
                    RecipeId = recipe.RecipeId,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients,
                    Instructions = recipe.Instructions,
                    ImageUrl = recipe.ImageUrl,
                    IsVegetarian = recipe.IsVegetarian,
                    Region = recipe.Region,
                    CreatedAt = recipe.CreatedAt,
                    UserId = recipe.UserId,
                    User = new UserDto
                    {
                        UserId = recipe.User.UserId,
                        Username = recipe.User.Name,
                        Email = recipe.User.Email
                    },
                    Feedbacks = recipe.Feedbacks.Select(f => new FeedbackDetailsDto
                    {
                        FeedbackId = f.FeedbackId,
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CreatedAt = f.CreatedAt
                    }).ToList()
                }).ToList();
                
                return Ok(recipeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




        // GET: api/recipe/{recipeName}
        [AllowAnonymous]
        [HttpGet("{recipeName}")]
        public async Task<IActionResult> GetRecipeByName(string recipeName)
        {
            try
            {
                var recipe = await _recipeRepository.GetRecipeByNameAsync(recipeName);
                if (recipe == null)
                    return NotFound("Recipe not found.");
                    
                var recipeDto = new RecipeDetailsDto
                {
                    RecipeId = recipe.RecipeId,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients,
                    Instructions = recipe.Instructions,
                    ImageUrl = recipe.ImageUrl,
                    IsVegetarian = recipe.IsVegetarian,
                    Region = recipe.Region,
                    CreatedAt = recipe.CreatedAt,
                    User = new UserDto
                    {
                        UserId = recipe.User.UserId,
                        Username = recipe.User.Name,
                        Email = recipe.User.Email
                    },
                    Feedbacks = recipe.Feedbacks.Select(f => new FeedbackDetailsDto
                    {
                        FeedbackId = f.FeedbackId,
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CreatedAt = f.CreatedAt
                    }).ToList()
                };

                return Ok(recipeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/recipe/{recipeName}
        [HttpPut("{recipeName}")]
        public async Task<IActionResult> UpdateRecipeByName(string recipeName, RecipeDto updatedRecipe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var recipe = await _recipeRepository.GetUserRecipeByNameAsync(recipeName, userId);

                if (recipe == null)
                    return NotFound("Recipe not found or not yours.");

                recipe.Title = updatedRecipe.Title;
                recipe.Description = updatedRecipe.Description;
                recipe.Ingredients = updatedRecipe.Ingredients;
                recipe.Instructions = updatedRecipe.Instructions;
                recipe.ImageUrl = updatedRecipe.ImageUrl;
                recipe.IsVegetarian = updatedRecipe.IsVegetarian;
                recipe.Region = updatedRecipe.Region;
                recipe.CreatedAt = DateTime.Now;

                await _recipeRepository.UpdateRecipeAsync(recipe);
                
                return Ok(new {message="Recipe Updated Successfully"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/recipe/{recipeName}
        [HttpDelete("{recipeName}")]
        public async Task<IActionResult> DeleteRecipeByName(string recipeName)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var recipe = await _recipeRepository.GetUserRecipeByNameAsync(recipeName, userId);

                if (recipe == null)
                    return NotFound("Recipe not found or not yours.");

                await _recipeRepository.DeleteRecipeAsync(recipe);
                
                return Ok(new { message = "Recipe deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // DELETE: api/recipe/admin/{recipeName}
        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/{recipeName}")]
        public async Task<IActionResult> DeleteAnyRecipeByName(string recipeName)
        {
            try
            {
                var recipe = await _recipeRepository.GetRecipeByNameAsync(recipeName);
                if (recipe == null)
                    return NotFound("Recipe not found.");
                    
                await _recipeRepository.DeleteRecipeAsync(recipe);

                return Ok(new { message = "Recipe deleted by admin." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/recipe/user/{userName}
        [AllowAnonymous]
        [HttpGet("user/{userName}")]
        public async Task<IActionResult> GetUserRecipesByName(string userName)
        {
            try
            {
                var recipes = await _recipeRepository.GetRecipeByUserNameAsync(userName);
                if (recipes == null || !recipes.Any())
                    return NotFound("No recipes found for this user.");

                var recipeDtos = recipes.Select(recipe => new RecipeDetailsDto
                {
                    RecipeId = recipe.RecipeId,
                    Title = recipe.Title,
                    Description = recipe.Description,
                    Ingredients = recipe.Ingredients,
                    Instructions = recipe.Instructions,
                    ImageUrl = recipe.ImageUrl,
                    IsVegetarian = recipe.IsVegetarian,
                    Region = recipe.Region,
                    CreatedAt = recipe.CreatedAt,
                    User = new UserDto
                    {
                        UserId = recipe.User.UserId,
                        Username = recipe.User.Name,
                        Email = recipe.User.Email
                    },
                    Feedbacks = recipe.Feedbacks.Select(f => new FeedbackDetailsDto
                    {
                        FeedbackId = f.FeedbackId,
                        Rating = f.Rating,
                        Comment = f.Comment,
                        CreatedAt = f.CreatedAt
                    }).ToList()
                }).ToList();

                return Ok(recipeDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
