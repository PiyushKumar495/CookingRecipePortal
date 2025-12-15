using CookingRecipePortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipePortal.Infrastructure
{
    public interface IRecipeRepository
    {
        Task AddRecipeAsync(Recipe recipe);
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<IEnumerable<Recipe>> FilterRecipesAsync(string? name, string? isVegetarian, string? region);
        Task<IEnumerable<Recipe>> GetRecipeByUserNameAsync(string userName);
        Task DeleteRecipeAsync(Recipe recipe);
        Task UpdateRecipeAsync(Recipe recipe);
        Task<Recipe?> GetRecipeByNameAsync(string recipeName);
        Task<Recipe?> GetUserRecipeByNameAsync(string recipeName, int userId);
        
    }


    public class RecipeRepository : IRecipeRepository
    {
        private readonly CookingDbContext _context;

        public RecipeRepository(CookingDbContext context)
        {
            _context = context;
        }

        public async Task AddRecipeAsync(Recipe recipe)
        {
            try
            {
                await _context.Recipes.AddAsync(recipe);
                _context.SaveChanges();
            }
            catch
            {
                throw new Exception("Failed to add recipe.");
            }
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            try
            {
                return await _context.Recipes
                    .Include(r => r.User)
                    .Include(r => r.Feedbacks)
                    .ToListAsync();
            }
            catch
            {
                throw new Exception("Failed to retrieve recipes.");
            }
        }

        public async Task<IEnumerable<Recipe>> FilterRecipesAsync(string? name, string? category, string? region)
        {
            try
            {
                var query = _context.Recipes
                    .Include(r => r.User)
                    .Include(r => r.Feedbacks)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(r => r.Title.Contains(name));

                if (!string.IsNullOrEmpty(category))
                {
                    if (category.ToLower() == "veg")
                        query = query.Where(r => r.IsVegetarian == true);
                    else if (category.ToLower() == "non-veg")
                        query = query.Where(r => r.IsVegetarian == false);
                }

                if (!string.IsNullOrEmpty(region))
                    query = query.Where(r => r.Region.Contains(region));

                return await query.ToListAsync();
            }
            catch
            {
                throw new Exception("Failed to filter recipes.");
            }
        }






        public async Task DeleteRecipeAsync(Recipe recipe)
        {
            try
            {
                _context.Recipes.Remove(recipe);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Failed to delete recipe.");
            }
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            try
            {
                _context.Recipes.Update(recipe);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Failed to update recipe.");
            }
        }


        public async Task<Recipe?> GetRecipeByNameAsync(string recipeName)
        {
            try
            {
                return await _context.Recipes
                    .Include(r => r.Feedbacks)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Title.ToLower() == recipeName.ToLower());
            }
            catch
            {
                throw new Exception("Failed to retrieve recipe by name.");
            }
        }

        public async Task<Recipe?> GetUserRecipeByNameAsync(string recipeName, int userId)
        {
            try
            {
                return await _context.Recipes
                    .Include(r => r.Feedbacks)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(r => r.Title.ToLower() == recipeName.ToLower() && r.UserId == userId);
            }
            catch
            {
                throw new Exception("Failed to retrieve user recipe by name.");
            }
        }

        public async Task<IEnumerable<Recipe>> GetRecipeByUserNameAsync(string userName)
        {
            try
            {
                return await _context.Recipes
                    .Include(r => r.Feedbacks)
                    .Include(r => r.User)
                    .Where(r => r.User.Name.ToLower() == userName.ToLower())
                    .ToListAsync();
            }
            catch
            {
                throw new Exception("Failed to retrieve recipes by user name.");
            }
        }

    }

}
