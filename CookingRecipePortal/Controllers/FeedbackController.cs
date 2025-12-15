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
    public class FeedbackController : ControllerBase
    {

        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackController(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        // POST: api/feedback
        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var recipe = await _feedbackRepository.GetRecipeByNameAsync(dto.RecipeName);
                if (recipe == null)
                    return NotFound("Recipe not found.");

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var feedback = new Feedback
                {
                    RecipeId = recipe.RecipeId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.Now
                };

                await _feedbackRepository.AddFeedbackAsync(feedback);
                return Ok(new { message = "Feedback submitted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




        // GET: api/feedback/recipe/{recipeName}
        [AllowAnonymous]
        [HttpGet("recipe/{recipeName}")]
        public async Task<IActionResult> GetFeedbacksForRecipe(string recipeName)
        {
            try
            {
                if (!await _feedbackRepository.RecipeExistsByNameAsync(recipeName))
                {
                    return NotFound("Recipe not found.");
                }

                var feedbacks = await _feedbackRepository.GetFeedbacksForRecipeByNameAsync(recipeName);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpDelete("{feedbackId}")]
        public async Task<IActionResult> DeleteFeedback(int feedbackId)
        {
            try
            {
                var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
                if (feedback == null)
                {
                    return NotFound("Feedback not found.");
                }
                // Check if the user is the owner of the feedback
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (feedback.UserId != userId)
                {
                    return Forbid("You can only delete your own feedback.");
                }
                await _feedbackRepository.DeleteFeedbackAsync(feedback);
                return Ok(new { message = "Feedback deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        
        [HttpPut("{feedbackId}")]
        public async Task<IActionResult> UpdateFeedback(int feedbackId, [FromBody] FeedbackDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
                if (feedback == null)
                {
                    return NotFound("Feedback not found.");
                }
                // Check if the user is the owner of the feedback
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                if (feedback.UserId != userId)
                {
                    return Forbid("You can only update your own feedback.");
                }
                feedback.Rating = dto.Rating;
                feedback.Comment = dto.Comment;
                
                await _feedbackRepository.UpdateFeednbackAsync(feedback);
                return Ok(new { message = "Feedback updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/{feedbackId}")]
        public async Task<IActionResult> DeleteAnyFeedback(int feedbackId)
        {
            try
            {
                var feedback = await _feedbackRepository.GetFeedbackByIdAsync(feedbackId);
                if (feedback == null)
                {
                    return NotFound("Feedback not found.");
                }
                
                await _feedbackRepository.DeleteFeedbackAsync(feedback);
                return Ok(new { message = "Feedback deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        
        [HttpGet("user/{userName}")]
        public async Task<IActionResult> GetMyFeedbacks(string userName)
        {
            try
            {
                var feedbacks = await _feedbackRepository.GetUserFeedbacksByNameAsync(userName);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
