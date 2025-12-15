using System.ComponentModel.DataAnnotations;

namespace CookingRecipePortal.DTO_s
{
    public class LoginDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_])[a-zA-Z\d\W_]{6,}$",
    ErrorMessage = "Password must be at least 6 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; }


    }
    public class LoginResponse
    {
        public int UserId { get; set; } = 0;
        public string Token { get; set; } = null!;
        public string Name { get; set; } = null!;

        public string Role { get; set; } = null!;
    }
}
