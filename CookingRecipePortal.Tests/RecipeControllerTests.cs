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
    public class RecipeControllerTests
    {
        private Mock<IRecipeRepository> _mockRecipeRepository;
        private RecipeController _controller;
        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _mockRecipeRepository = new Mock<IRecipeRepository>();
            _controller = new RecipeController(_mockRecipeRepository.Object);
            
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

        #region AddRecipe Tests

        [Test]
        public async Task AddRecipe_ValidRecipe_ReturnsOkResult()
        {
            // Arrange
            var recipeDto = new RecipeDto
            {
                Title = "Test Recipe",
                Description = "Test Description",
                Ingredients = "Test Ingredients",
                Instructions = "Test Instructions",
                IsVegetarian = true,
                Region = "Test Region"
            };

            _mockRecipeRepository.Setup(x => x.AddRecipeAsync(It.IsAny<Recipe>()))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.AddRecipe(recipeDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task AddRecipe_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var recipeDto = new RecipeDto();
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.AddRecipe(recipeDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        #endregion

        #region GetAllRecipes Tests

        [Test]
        public async Task GetAllRecipes_ValidRequest_ReturnsOkResult()
        {
            // Arrange
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    RecipeId = 1,
                    Title = "Test Recipe",
                    User = new User { UserId = 1, Name = "Test User", Email = "test@test.com" },
                    Feedbacks = new List<Feedback>()
                }
            };

            _mockRecipeRepository.Setup(x => x.GetAllRecipesAsync())
                                .ReturnsAsync(recipes);

            // Act
            var result = await _controller.GetAllRecipes();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetAllRecipes_RepositoryException_ReturnsInternalServerError()
        {
            // Arrange
            _mockRecipeRepository.Setup(x => x.GetAllRecipesAsync())
                                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllRecipes();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);
        }

        #endregion

        #region GetRecipeByName Tests

        [Test]
        public async Task GetRecipeByName_ValidRecipe_ReturnsOkResult()
        {
            // Arrange
            var recipe = new Recipe
            {
                RecipeId = 1,
                Title = "Test Recipe",
                User = new User { UserId = 1, Name = "Test User", Email = "test@test.com" },
                Feedbacks = new List<Feedback>()
            };

            _mockRecipeRepository.Setup(x => x.GetRecipeByNameAsync("Test Recipe"))
                                .ReturnsAsync(recipe);

            // Act
            var result = await _controller.GetRecipeByName("Test Recipe");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetRecipeByName_RecipeNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRecipeRepository.Setup(x => x.GetRecipeByNameAsync("Nonexistent Recipe"))
                                .ReturnsAsync((Recipe)null);

            // Act
            var result = await _controller.GetRecipeByName("Nonexistent Recipe");

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        #endregion

        #region UpdateRecipeByName Tests

        [Test]
        public async Task UpdateRecipeByName_ValidOwner_ReturnsOkResult()
        {
            // Arrange
            var recipeDto = new RecipeDto
            {
                Title = "Updated Recipe",
                Description = "Updated Description",
                Ingredients = "Updated Ingredients",
                Instructions = "Updated Instructions",
                IsVegetarian = false,
                Region = "Updated Region"
            };

            var recipe = new Recipe { RecipeId = 1, Title = "Test Recipe", UserId = 1 };

            _mockRecipeRepository.Setup(x => x.GetUserRecipeByNameAsync("Test Recipe", 1))
                                .ReturnsAsync(recipe);
            _mockRecipeRepository.Setup(x => x.UpdateRecipeAsync(recipe))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateRecipeByName("Test Recipe", recipeDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task UpdateRecipeByName_RecipeNotFound_ReturnsNotFound()
        {
            // Arrange
            var recipeDto = new RecipeDto
            {
                Title = "Updated Recipe",
                Description = "Updated Description",
                Ingredients = "Updated Ingredients",
                Instructions = "Updated Instructions",
                IsVegetarian = false,
                Region = "Updated Region"
            };

            _mockRecipeRepository.Setup(x => x.GetUserRecipeByNameAsync("Nonexistent Recipe", 1))
                                .ReturnsAsync((Recipe)null);

            // Act
            var result = await _controller.UpdateRecipeByName("Nonexistent Recipe", recipeDto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        #endregion

        #region DeleteRecipeByName Tests

        [Test]
        public async Task DeleteRecipeByName_ValidOwner_ReturnsOkResult()
        {
            // Arrange
            var recipe = new Recipe { RecipeId = 1, Title = "Test Recipe", UserId = 1 };

            _mockRecipeRepository.Setup(x => x.GetUserRecipeByNameAsync("Test Recipe", 1))
                                .ReturnsAsync(recipe);
            _mockRecipeRepository.Setup(x => x.DeleteRecipeAsync(recipe))
                                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteRecipeByName("Test Recipe");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task DeleteRecipeByName_RecipeNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockRecipeRepository.Setup(x => x.GetUserRecipeByNameAsync("Nonexistent Recipe", 1))
                                .ReturnsAsync((Recipe)null);

            // Act
            var result = await _controller.DeleteRecipeByName("Nonexistent Recipe");

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        #endregion

        #region FilterRecipes Tests

        [Test]
        public async Task FilterRecipes_ValidFilters_ReturnsOkResult()
        {
            // Arrange
            var recipes = new List<Recipe>
            {
                new Recipe
                {
                    RecipeId = 1,
                    Title = "Vegetarian Recipe",
                    IsVegetarian = true,
                    User = new User { UserId = 1, Name = "Test User", Email = "test@test.com" },
                    Feedbacks = new List<Feedback>()
                }
            };

            _mockRecipeRepository.Setup(x => x.FilterRecipesAsync("Vegetarian", null, null))
                                .ReturnsAsync(recipes);

            // Act
            var result = await _controller.FilterRecipes("Vegetarian", null, null);

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