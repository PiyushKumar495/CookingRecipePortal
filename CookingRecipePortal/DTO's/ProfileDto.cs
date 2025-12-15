using System.ComponentModel.DataAnnotations;

namespace CookingRecipePortal.DTO_s
{
    public class UpdateProfileDto
    {
        [Required]
        public string Name { get; set; } = null!;
        
        public string? Bio { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; } = null!;
        
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}