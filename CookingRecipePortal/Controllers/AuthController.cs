using CookingRecipePortal.DTO_s;
using CookingRecipePortal.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace CookingRecipePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        

        public AuthController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
           
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var registeredUser = await _userRepository.RegisterAsync(user);

                return Ok(new
                {
                    message = "User registered successfully. Please login to continue.",
                    user = new
                    {
                        registeredUser.UserId,
                        registeredUser.Name,
                        registeredUser.Email,
                        registeredUser.Role
                    }
                });
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("User with same Email already exists"))
                {
                    return BadRequest(new { message = ex.Message });
                }

                return StatusCode(500, new { message = "An unexpected error occurred." });
            }
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var loggedInUser = await _userRepository.LoginAsync(user);
                if (loggedInUser == null)
                    return Unauthorized(new { message = "Invalid Email or password." });
                return Ok(new { message = "Login Successful ", loggedInUser });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }



    }
}
