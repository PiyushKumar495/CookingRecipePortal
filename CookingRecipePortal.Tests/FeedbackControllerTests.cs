using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using CookingRecipePortal.Controllers;
using CookingRecipePortal.Infrastructure;
using CookingRecipePortal.DTO_s;
using CookingRecipePortal.Models;

namespace CookingRecipePortal.Tests
{
    [TestFixture]
    public class FeedbackControllerTests
    {
        private Mock<IFeedbackRepository> _mockFeedbackRepository;
        private FeedbackController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _mockFeedbackRepository = new Mock<IFeedbackRepository>();
            _controller = new FeedbackController(_mockFeedbackRepository.Object);
            
            _user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Role, "User")
            }));
            
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _user }
            };
        }

        #region AddFeedback Tests

        [Test]
        public async Task AddFeedback_ValidFeedback_ReturnsOkResult()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                RecipeName = "Test Recipe",
                Rating = 5,
                Comment = "Great recipe!"
            };

            var recipe = new Recipe { RecipeId = 1, Title = "Test Recipe" };

            _mockFeedbackRepository.Setup(x => x.GetRecipeByNameAsync("Test Recipe"))
                                  .ReturnsAsync(recipe);
            _mockFeedbackRepository.Setup(x => x.AddFeedbackAsync(It.IsAny<Feedback>()))
                                  .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddFeedback(feedbackDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task AddFeedback_RecipeNotFound_ReturnsNotFound()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                RecipeName = "Nonexistent Recipe",
                Rating = 5,
                Comment = "Great recipe!"
            };

            _mockFeedbackRepository.Setup(x => x.GetRecipeByNameAsync("Nonexistent Recipe"))
                                  .ReturnsAsync((Recipe)null);

            // Act
            var result = await _controller.AddFeedback(feedbackDto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task AddFeedback_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var feedbackDto = new FeedbackDto();
            _controller.ModelState.AddModelError("Rating", "Rating is required");

            // Act
            var result = await _controller.AddFeedback(feedbackDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        #endregion

        #region GetFeedbacksForRecipe Tests

        [Test]
        public async Task GetFeedbacksForRecipe_ValidRecipe_ReturnsOkResult()
        {
            // Arrange
            var feedbacks = new List<object> { new { Rating = 5, Comment = "Great!" } };

            _mockFeedbackRepository.Setup(x => x.RecipeExistsByNameAsync("Test Recipe"))
                                  .ReturnsAsync(true);
            _mockFeedbackRepository.Setup(x => x.GetFeedbacksForRecipeByNameAsync("Test Recipe"))
                                  .ReturnsAsync(feedbacks);

            // Act
            var result = await _controller.GetFeedbacksForRecipe("Test Recipe");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetFeedbacksForRecipe_RecipeNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockFeedbackRepository.Setup(x => x.RecipeExistsByNameAsync("Nonexistent Recipe"))
                                  .ReturnsAsync(false);

            // Act
            var result = await _controller.GetFeedbacksForRecipe("Nonexistent Recipe");

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        #endregion

        #region DeleteFeedback Tests

        [Test]
        public async Task DeleteFeedback_ValidOwner_ReturnsOkResult()
        {
            // Arrange
            var feedback = new Feedback { FeedbackId = 1, UserId = 1 };

            _mockFeedbackRepository.Setup(x => x.GetFeedbackByIdAsync(1))
                                  .ReturnsAsync(feedback);
            _mockFeedbackRepository.Setup(x => x.DeleteFeedbackAsync(feedback))
                                  .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteFeedback(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task DeleteFeedback_NotOwner_ReturnsForbid()
        {
            // Arrange
            var feedback = new Feedback { FeedbackId = 1, UserId = 2 };

            _mockFeedbackRepository.Setup(x => x.GetFeedbackByIdAsync(1))
                                  .ReturnsAsync(feedback);

            // Act
            var result = await _controller.DeleteFeedback(1);

            // Assert
            Assert.IsInstanceOf<ForbidResult>(result);
        }

        [Test]
        public async Task DeleteFeedback_FeedbackNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockFeedbackRepository.Setup(x => x.GetFeedbackByIdAsync(1))
                                  .ReturnsAsync((Feedback)null);

            // Act
            var result = await _controller.DeleteFeedback(1);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        #endregion

        #region UpdateFeedback Tests

        [Test]
        public async Task UpdateFeedback_ValidOwner_ReturnsOkResult()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                RecipeName = "Test Recipe",
                Rating = 4,
                Comment = "Updated comment"
            };

            var feedback = new Feedback { FeedbackId = 1, UserId = 1 };

            _mockFeedbackRepository.Setup(x => x.GetFeedbackByIdAsync(1))
                                  .ReturnsAsync(feedback);
            _mockFeedbackRepository.Setup(x => x.UpdateFeednbackAsync(feedback))
                                  .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateFeedback(1, feedbackDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task UpdateFeedback_NotOwner_ReturnsForbid()
        {
            // Arrange
            var feedbackDto = new FeedbackDto
            {
                RecipeName = "Test Recipe",
                Rating = 4,
                Comment = "Updated comment"
            };

            var feedback = new Feedback { FeedbackId = 1, UserId = 2 };

            _mockFeedbackRepository.Setup(x => x.GetFeedbackByIdAsync(1))
                                  .ReturnsAsync(feedback);

            // Act
            var result = await _controller.UpdateFeedback(1, feedbackDto);

            // Assert
            Assert.IsInstanceOf<ForbidResult>(result);
        }

        #endregion

        [TearDown]
        public void TearDown()
        {
            // Cleanup if needed
        }
    }
}