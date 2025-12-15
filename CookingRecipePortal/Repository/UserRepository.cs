using CookingRecipePortal.DTO_s;
using CookingRecipePortal.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CookingRecipePortal.Infrastructure
{
    public interface IUserRepository
    {
        Task<User> RegisterAsync(RegisterDto user);
        Task<LoginResponse> LoginAsync(LoginDto request);


    }

    public class UserRepository : IUserRepository
    {
        private readonly CookingDbContext _context;
        private readonly IConfiguration _configuration;
        public UserRepository(CookingDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User> RegisterAsync(RegisterDto user)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                throw new Exception("User with same Email already exists");
            }

            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password),
                Bio = user.Bio,
                CreatedAt = DateTime.UtcNow,
                Role = "User" // Always default to User role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<LoginResponse> LoginAsync(LoginDto request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
                if (user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    var token = GenerateToken(user);
                    var loginResponse = new LoginResponse
                    {
                        Token = token,
                        UserId = user.UserId,
                        Name = user.Name,
                        Role= user.Role
                    };
                    return loginResponse;
                }
                else
                {
                    throw new Exception("Invalid Email or password");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while logging in", ex);
            }
        }
        private string GenerateToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
