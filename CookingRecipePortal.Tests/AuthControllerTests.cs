using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using CookingRecipePortal.Controllers;
using CookingRecipePortal.Infrastructure;
using CookingRecipePortal.DTO_s;
using CookingRecipePortal.Models;

namespace CookingRecipePortal.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IUserRepository> _mockUserRepository;
        private AuthController _controller;

        [SetUp]
        public void Setup()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _controller = new AuthController(_mockUserRepository.Object);
        }

        #region Register Tests

        [Test]
        public async Task Register_ValidUser_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                Bio = "Test bio"
            };

            var user = new User
            {
                UserId = 1,
                Name = "John Doe",
                Email = "john@example.com",
                Role = "User"
            };

            _mockUserRepository.Setup(x => x.RegisterAsync(registerDto))
                              .ReturnsAsync(user);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult?.Value);
        }

        [Test]
        public async Task Register_DuplicateEmail_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "John Doe",
                Email = "existing@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _mockUserRepository.Setup(x => x.RegisterAsync(registerDto))
                              .ThrowsAsync(new Exception("User with same Email already exists"));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Register_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var registerDto = new RegisterDto();
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Register_UnexpectedException_ReturnsInternalServerError()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Name = "John Doe",
                Email = "john@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _mockUserRepository.Setup(x => x.RegisterAsync(registerDto))
                              .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);
        }

        #endregion

        #region Login Tests

        [Test]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "john@example.com",
                Password = "Password123!"
            };

            var loginResponse = new LoginResponse
            {
                UserId = 1,
                Token = "jwt-token",
                Name = "John Doe",
                Role = "User"
            };

            _mockUserRepository.Setup(x => x.LoginAsync(loginDto))
                              .ReturnsAsync(loginResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult?.Value);
        }

        [Test]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "john@example.com",
                Password = "WrongPassword123!"
            };

            _mockUserRepository.Setup(x => x.LoginAsync(loginDto))
                              .ReturnsAsync((LoginResponse)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<UnauthorizedObjectResult>(result);
        }

        [Test]
        public async Task Login_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            var loginDto = new LoginDto();
            _controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task Login_RepositoryException_ReturnsInternalServerError()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Email = "john@example.com",
                Password = "Password123!"
            };

            _mockUserRepository.Setup(x => x.LoginAsync(loginDto))
                              .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(500, objectResult?.StatusCode);
        }

        #endregion

        
    }
}