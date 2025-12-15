using System;
using System.Collections.Generic;

namespace CookingRecipePortal.Models;

public partial class Feedback
{
    public int FeedbackId { get; set; }

    public int? Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int UserId { get; set; }

    public int RecipeId { get; set; }

    public virtual Recipe Recipe { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
