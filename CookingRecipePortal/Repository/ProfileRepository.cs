using CookingRecipePortal.DTO_s;
using CookingRecipePortal.Models;
using Microsoft.EntityFrameworkCore;

namespace CookingRecipePortal.Infrastructure
{
    public interface IProfileRepository
    {
        Task<object?> GetUserProfile(int userId);
        Task UpdateUserAsync(int userId, UpdateProfileDto profile);
        Task ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    }

    public class ProfileRepository : IProfileRepository
    {

        private readonly CookingDbContext _context;

        public ProfileRepository(CookingDbContext context)
        {
            _context = context;
        }

        public async Task<object?> GetUserProfile(int userId)
        {

            try
            {
                return await _context.Users
                .Where(u => u.UserId == userId)
                .Select(u => new
                {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.Bio,
                    u.CreatedAt
                }).FirstOrDefaultAsync();
            }
            catch
            {
                throw new Exception("Failed to retrieve user profile.");
            }

        }

        public async Task UpdateUserAsync(int userId, UpdateProfileDto profile)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                    throw new Exception("User not found.");

                user.Name = profile.Name;
                user.Bio = profile.Bio;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw new Exception("Failed to update user details.");
            }
        }

        public async Task ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
                throw new Exception("User not found.");

            if (user.PasswordHash != oldPassword)
                throw new Exception("Old password is incorrect.");

            user.PasswordHash = newPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

    }
}
