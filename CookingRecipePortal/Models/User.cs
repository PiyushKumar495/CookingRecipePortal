using System;
using System.Collections.Generic;

namespace CookingRecipePortal.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Bio { get; set; }

    public DateTime? CreatedAt { get; set; }
    public string Role { get; set; } = "User"; 

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
}
