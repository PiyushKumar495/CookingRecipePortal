using CookingRecipePortal.DTO_s;
using CookingRecipePortal.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CookingRecipePortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors]
    public class ProfileController : ControllerBase
    {

        private readonly IProfileRepository _profileRepository;

        public ProfileController(IProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }


        // GET: api/profile
        [HttpGet]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var profile = await _profileRepository.GetUserProfile(userId);

                if (profile == null)
                    return NotFound("User not found.");

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // PUT: api/profile
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                await _profileRepository.UpdateUserAsync(userId, profileDto);
                var updatedProfile = await _profileRepository.GetUserProfile(userId);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        // [HttpPut("change-password/{userId}")]
        // public async Task<IActionResult> ChangePassword(int userId, [FromBody] ChangePasswordDto dto)
        // {
        //     try
        //     {
        //         await _profileRepository.ChangePasswordAsync(userId, dto.OldPassword, dto.NewPassword);
        //         return Ok(new { message = "Password changed successfully." });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new { error = ex.Message });
        //     }
        // }

    }
}
