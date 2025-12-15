using System;
using System.Security.Claims;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CookingRecipePortal.Controllers;
using CookingRecipePortal.Infrastructure;
using CookingRecipePortal.DTO_s;

namespace CookingRecipePortal.Tests
{
    [TestFixture]
    public class ProfileControllerTests
    {
        private Mock<IProfileRepository> _mockProfileRepository;
        private ProfileController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _mockProfileRepository = new Mock<IProfileRepository>();
            _controller = new ProfileController(_mockProfileRepository.Object);
            
            _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };
        }

        #region GetMyProfile Tests

        [Test]
        public async Task GetMyProfile_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var profileData = new
            {
                UserId = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Bio = "Test bio",
                CreatedAt = DateTime.UtcNow
            };

            _mockProfileRepository.Setup(x => x.GetUserProfile(1))
                                 .ReturnsAsync(profileData);

            // Act
            var result = await _controller.GetMyProfile();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(profileData, okResult?.Value);
        }

        [Test]
        public async Task GetMyProfile_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockProfileRepository.Setup(x => x.GetUserProfile(1))
                                 .ReturnsAsync((object)null);

            // Act
            var result = await _controller.GetMyProfile();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetMyProfile_RepositoryException_ReturnsInternalServerError()
        {
            // Arrange
            _mockProfileRepository.Setup(x => x.GetUserProfile(1))
                                 .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetMyProfile();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);
        }

        #endregion

        #region UpdateUser Tests

        [Test]
        public async Task UpdateUser_ValidProfile_ReturnsOkResult()
        {
            // Arrange
            var updateDto = new UpdateProfileDto
            {
                Name = "Updated Name",
                Bio = "Updated bio"
            };

            var updatedProfile = new
            {
                UserId = 1,
                Name = "Updated Name",
                Email = "john@example.com",
                Bio = "Updated bio",
                CreatedAt = DateTime.UtcNow
            };

            _mockProfileRepository.Setup(x => x.UpdateUserAsync(1, updateDto))
                                 .Returns(Task.CompletedTask);
            
            _mockProfileRepository.Setup(x => x.GetUserProfile(1))
                                 .ReturnsAsync(updatedProfile);

            // Act
            var result = await _controller.UpdateUser(updateDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(updatedProfile, okResult?.Value);
        }

        [Test]
        public async Task UpdateUser_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var updateDto = new UpdateProfileDto();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.UpdateUser(updateDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateUser_RepositoryException_ReturnsInternalServerError()
        {
            // Arrange
            var updateDto = new UpdateProfileDto
            {
                Name = "Updated Name",
                Bio = "Updated bio"
            };

            _mockProfileRepository.Setup(x => x.UpdateUserAsync(1, updateDto))
                                 .ThrowsAsync(new Exception("Update failed"));

            // Act
            var result = await _controller.UpdateUser(updateDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);
        }

        [Test]
        public async Task UpdateUser_NullBio_ReturnsOkResult()
        {
            // Arrange
            var updateDto = new UpdateProfileDto
            {
                Name = "Updated Name",
                Bio = null
            };

            var updatedProfile = new
            {
                UserId = 1,
                Name = "Updated Name",
                Email = "john@example.com",
                Bio = (string)null,
                CreatedAt = DateTime.UtcNow
            };

            _mockProfileRepository.Setup(x => x.UpdateUserAsync(1, updateDto))
                                 .Returns(Task.CompletedTask);
            
            _mockProfileRepository.Setup(x => x.GetUserProfile(1))
                                 .ReturnsAsync(updatedProfile);

            // Act
            var result = await _controller.UpdateUser(updateDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            // Cleanup if needed
        }
    }
}