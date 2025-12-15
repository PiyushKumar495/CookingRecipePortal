using System;
using System.Collections.Generic;

namespace CookingRecipePortal.Models;

public partial class Recipe
{
    public int RecipeId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }
    public string Ingredients { get; set; }
    public string Instructions { get; set; }
    public string? ImageUrl { get; set; } // Nullable image URL

    public bool IsVegetarian { get; set; }

    public string? Region { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual User User { get; set; } = null!;
}
