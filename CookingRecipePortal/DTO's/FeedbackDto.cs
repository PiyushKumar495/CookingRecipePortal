using System.ComponentModel.DataAnnotations;

namespace CookingRecipePortal.DTO_s
{
    public class FeedbackDto
    {
        [Required]
        public string RecipeName { get; set; } = null!;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [StringLength(200, ErrorMessage = "Comment cannot be longer than 200 characters.")]
        public string? Comment { get; set; }
    }
    public class UserFeedbackDto
    {
        public int FeedbackId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string RecipeTitle { get; set; } = null!;
    }

}
