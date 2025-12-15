using CookingRecipePortal.DTO_s;
using CookingRecipePortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipePortal.Infrastructure
{
    public interface IFeedbackRepository
    {
        Task<bool> RecipeExistsAsync(int recipeId);
        Task<bool> RecipeExistsByNameAsync(string recipeName);
        Task<Recipe?> GetRecipeByNameAndUserAsync(string recipeName, string userName);
        Task<Recipe?> GetRecipeByNameAsync(string recipeName);
        Task AddFeedbackAsync(Feedback feedback);
        Task<IEnumerable<object>> GetFeedbacksForRecipeAsync(int recipeId);
        Task<IEnumerable<object>> GetFeedbacksForRecipeByNameAsync(string recipeName);
        Task<Feedback> GetFeedbackByIdAsync(int feedbackId);
        Task<IEnumerable<UserFeedbackDto>> GetUserFeedbacksAsync(int userId);
        Task<IEnumerable<UserFeedbackDto>> GetUserFeedbacksByNameAsync(string userName);
        Task DeleteFeedbackAsync(Feedback feedback);
        Task UpdateFeednbackAsync(Feedback feedback);
    }

    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly CookingDbContext _context;
        public FeedbackRepository(CookingDbContext context)
        {
            _context = context;
        }
        public async Task<bool> RecipeExistsAsync(int recipeId)
        {

            try
            {
                return await _context.Recipes.AnyAsync(r => r.RecipeId == recipeId);
            }
            catch
            {
                throw new Exception("Failed to check if recipe exists.");
            }

        }
        public async Task AddFeedbackAsync(Feedback feedback)
        {

            try
            {
                await _context.Feedbacks.AddAsync(feedback);
                _context.SaveChanges(); 
            }
            catch
            {
                throw new Exception("Failed to add feedback.");
            }

        }
        public async Task<IEnumerable<object>> GetFeedbacksForRecipeAsync(int recipeId)
        {
            return await _context.Feedbacks
                .Where(f => f.RecipeId == recipeId)
                .Select(f => new
                {
                    f.FeedbackId,
                    f.Rating,
                    f.Comment,
                    UserName = f.User.Name,
                    f.CreatedAt
                }).ToListAsync();
        }
        public async Task DeleteFeedbackAsync(Feedback feedback)
        {
            try
            {
                _context.Feedbacks.Remove(feedback);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Failed to delete feedback.");
            }
        }
        public async Task UpdateFeednbackAsync(Feedback feedback)
        {
            try
            {
                _context.Feedbacks.Update(feedback);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Failed to update feedback.");
            }
        }
        public async Task<Feedback> GetFeedbackByIdAsync(int feedbackId)
        {
            try
            {
                return await _context.Feedbacks.FindAsync(feedbackId);
            }
            catch
            {
                throw new Exception("Failed to retrieve feedback.");
            }
        }
        public async Task<IEnumerable<UserFeedbackDto>> GetUserFeedbacksAsync(int userId)
        {
            return await _context.Feedbacks
                .Where(f => f.UserId == userId)
                .Include(f => f.Recipe)
                .Select(f => new UserFeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt,
                    RecipeTitle = f.Recipe.Title
                })
                .ToListAsync();
        }

        public async Task<bool> RecipeExistsByNameAsync(string recipeName)
        {
            try
            {
                return await _context.Recipes.AnyAsync(r => r.Title.ToLower() == recipeName.ToLower());
            }
            catch
            {
                throw new Exception("Failed to check if recipe exists by name.");
            }
        }

        public async Task<IEnumerable<object>> GetFeedbacksForRecipeByNameAsync(string recipeName)
        {
            return await _context.Feedbacks
                .Where(f => f.Recipe.Title.ToLower() == recipeName.ToLower())
                .Select(f => new
                {
                    f.FeedbackId,
                    f.Rating,
                    f.Comment,
                    UserName = f.User.Name,
                    f.CreatedAt
                }).ToListAsync();
        }

        public async Task<IEnumerable<UserFeedbackDto>> GetUserFeedbacksByNameAsync(string userName)
        {
            return await _context.Feedbacks
                .Where(f => f.User.Name.ToLower() == userName.ToLower())
                .Include(f => f.Recipe)
                .Select(f => new UserFeedbackDto
                {
                    FeedbackId = f.FeedbackId,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt,
                    RecipeTitle = f.Recipe.Title
                })
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeByNameAndUserAsync(string recipeName, string userName)
        {
            return await _context.Recipes
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Title.ToLower() == recipeName.ToLower() && r.User.Name.ToLower() == userName.ToLower());
        }

        public async Task<Recipe?> GetRecipeByNameAsync(string recipeName)
        {
            return await _context.Recipes
                .FirstOrDefaultAsync(r => r.Title.ToLower() == recipeName.ToLower());
        }

    }

}
