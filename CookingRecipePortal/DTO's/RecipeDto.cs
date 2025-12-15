using System.ComponentModel.DataAnnotations;

namespace CookingRecipePortal.DTO_s
{
    public class RecipeDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters.")]
        public string? Description { get; set; }

        [Required]
        public bool IsVegetarian { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Cuisine cannot be longer than 50 characters.")]
        public string? Region { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Ingredients cannot be longer than 500 characters.")]
        public string Ingredients { get; set; }

        [Required]
        [StringLength(1500, ErrorMessage = "Instructions cannot be longer than 1500 characters.")]
        public string Instructions { get; set; }


        public string? ImageUrl { get; set; } // Property for recipe image
    }
    public class RecipeDetailsDto
    {
        public int RecipeId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string Ingredients { get; set; } = null!;
        public string Instructions { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public bool IsVegetarian { get; set; }
        public string? Region { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int UserId { get; set; }
        public UserDto User { get; set; } = null!;
        public List<FeedbackDetailsDto> Feedbacks { get; set; } = new();
    }

    public class FeedbackDetailsDto
    {
        public int FeedbackId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
    public class UserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        // Add other properties as needed
    }

}
