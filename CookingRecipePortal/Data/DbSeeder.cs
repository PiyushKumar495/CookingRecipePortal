using CookingRecipePortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipePortal.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(CookingDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed Users
            if (!await context.Users.AnyAsync())
            {
                var users = new List<User>
                {
                    new User { Name = "Admin", Email = "admin@cookingportal.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"), Role = "Admin", Bio = "System Administrator", CreatedAt = DateTime.Now },
                    new User { Name = "Rajesh Kumar", Email = "rajesh@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"), Role = "User", Bio = "Traditional Indian chef from Delhi", CreatedAt = DateTime.Now },
                    new User { Name = "Priya Sharma", Email = "priya@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"), Role = "User", Bio = "Home cook specializing in South Indian cuisine", CreatedAt = DateTime.Now },
                    new User { Name = "Amit Patel", Email = "amit@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"), Role = "User", Bio = "Food blogger and Gujarati cuisine expert", CreatedAt = DateTime.Now }
                };
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
            }

            // Seed Recipes
            if (!await context.Recipes.AnyAsync())
            {
                var rajesh = await context.Users.FirstAsync(u => u.Email == "rajesh@example.com");
                var priya = await context.Users.FirstAsync(u => u.Email == "priya@example.com");
                var amit = await context.Users.FirstAsync(u => u.Email == "amit@example.com");

                var recipes = new List<Recipe>
                {
                    new Recipe { Title = "Butter Chicken", Description = "Rich and creamy North Indian curry", Ingredients = "Chicken, Butter, Tomatoes, Cream, Garam Masala, Onions, Garlic, Ginger", Instructions = "1. Marinate chicken in yogurt 2. Cook chicken 3. Make tomato base 4. Add cream and spices 5. Simmer together", IsVegetarian = false, ImageUrl = "https://cheflolaskitchen.com/wp-content/uploads/2023/04/Butter-Chicken-9-1-745x1024.jpg.webp", Region = "North Indian", UserId = rajesh.UserId, CreatedAt = DateTime.Now },
                    new Recipe { Title = "Masala Dosa", Description = "Crispy South Indian crepe with spiced potato filling", Ingredients = "Rice, Urad Dal, Potatoes, Onions, Mustard Seeds, Curry Leaves, Turmeric", Instructions = "1. Soak rice and dal 2. Grind to batter 3. Ferment overnight 4. Make potato masala 5. Cook dosa and fill", IsVegetarian = true, ImageUrl = "https://i.pinimg.com/736x/2a/c1/51/2ac15169850a766390adcba3a6d28eef.jpg", Region = "South Indian", UserId = priya.UserId, CreatedAt = DateTime.Now },
                    new Recipe { Title = "Biryani", Description = "Fragrant basmati rice with spiced meat", Ingredients = "Basmati Rice, Mutton, Yogurt, Saffron, Fried Onions, Mint, Coriander, Whole Spices", Instructions = "1. Soak rice 2. Marinate meat 3. Cook meat 4. Layer rice and meat 5. Dum cook for 45 minutes", IsVegetarian = false, ImageUrl = "https://www.whiskaffair.com/wp-content/uploads/2020/07/Chicken-Biryani-2-3.jpg", Region = "Hyderabadi", UserId = amit.UserId, CreatedAt = DateTime.Now },
                    new Recipe { Title = "Dhokla", Description = "Steamed Gujarati snack made from gram flour", Ingredients = "Gram Flour, Yogurt, Ginger, Green Chili, Eno, Mustard Seeds, Curry Leaves", Instructions = "1. Mix batter 2. Add Eno 3. Steam for 15 minutes 4. Prepare tempering 5. Pour over dhokla", IsVegetarian = true, ImageUrl = "https://i.cdn.newsbytesapp.com/images/l198_8311591421085.jpg", Region = "Gujarati", UserId = rajesh.UserId, CreatedAt = DateTime.Now }
                };
                context.Recipes.AddRange(recipes);
                await context.SaveChangesAsync();
            }

            // Seed Feedback
            if (!await context.Feedbacks.AnyAsync())
            {
                var rajesh = await context.Users.FirstAsync(u => u.Email == "rajesh@example.com");
                var priya = await context.Users.FirstAsync(u => u.Email == "priya@example.com");
                var amit = await context.Users.FirstAsync(u => u.Email == "amit@example.com");

                var butterChicken = await context.Recipes.FirstAsync(r => r.Title == "Butter Chicken");
                var dosa = await context.Recipes.FirstAsync(r => r.Title == "Masala Dosa");
                var biryani = await context.Recipes.FirstAsync(r => r.Title == "Biryani");
                var dhokla = await context.Recipes.FirstAsync(r => r.Title == "Dhokla");

                var feedbacks = new List<Feedback>
                {
                    new Feedback { Rating = 5, Comment = "Perfect butter chicken! Restaurant quality at home.", UserId = priya.UserId, RecipeId = butterChicken.RecipeId, CreatedAt = DateTime.Now },
                    new Feedback { Rating = 4, Comment = "Great dosa recipe, but fermentation took longer in winter.", UserId = amit.UserId, RecipeId = dosa.RecipeId, CreatedAt = DateTime.Now },
                    new Feedback { Rating = 5, Comment = "Best biryani recipe ever! The dum cooking technique is perfect.", UserId = rajesh.UserId, RecipeId = biryani.RecipeId, CreatedAt = DateTime.Now },
                    new Feedback { Rating = 4, Comment = "Soft and spongy dhokla. Added extra green chilies for more heat.", UserId = priya.UserId, RecipeId = dhokla.RecipeId, CreatedAt = DateTime.Now },
                    new Feedback { Rating = 5, Comment = "Authentic taste! Reminds me of my grandmother's cooking.", UserId = amit.UserId, RecipeId = butterChicken.RecipeId, CreatedAt = DateTime.Now }
                };
                context.Feedbacks.AddRange(feedbacks);
                await context.SaveChangesAsync();
            }
        }
    }
}