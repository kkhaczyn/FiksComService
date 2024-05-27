using System;
using FiksComService.Controllers;
using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace FiksComService.Tests
{
    public class AccountControllerTests
    {
        private AccountController _accountController;
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<SignInManager<User>> _mockSignInManager;
        private Mock<RoleManager<Role>> _mockRoleManager;
        private Mock<ILogger<AccountController>> _mockLogger;
        private Mock<IOrderDetailRepository> _mockOrderDetailRepository;

        [SetUp]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>();
            _mockRoleManager = new Mock<RoleManager<Role>>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _mockOrderDetailRepository = new Mock<IOrderDetailRepository>();

            _accountController = new AccountController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockRoleManager.Object,
                _mockLogger.Object,
                _mockOrderDetailRepository.Object);
        }

        [Test]
        public async Task SignUp_ValidRequest_ReturnsOk()
        {
            // Arrange
            SignUpRequest signUpRequest = new SignUpRequest
            {
                UserName = "testUser",
                Email = "test@example.com",
                PhoneNumber = "123456789",
                Password = "password"
            };

            _mockUserManager.Setup(m => m.FindByNameAsync(signUpRequest.UserName)).ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.FindByEmailAsync(signUpRequest.Email)).ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), signUpRequest.Password)).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(m => m.FindByNameAsync(signUpRequest.UserName)).ReturnsAsync(new User { UserName = signUpRequest.UserName });

            // Act
            var result = await _accountController.SignUp(signUpRequest);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            Assert.AreEqual("Zarejestrowano się pomyślnie", ((OkObjectResult)result).Value);
        }

        [Test]
        public async Task SignUp_UserWithNameExists_ReturnsBadRequest()
        {
            // Arrange
            SignUpRequest signUpRequest = new SignUpRequest
            {
                UserName = "existingUser",
                Email = "new@example.com",
                PhoneNumber = "123456789",
                Password = "password"
            };

            _mockUserManager.Setup(m => m.FindByNameAsync(signUpRequest.UserName)).ReturnsAsync(new User { UserName = signUpRequest.UserName });

            // Act
            var result = await _accountController.SignUp(signUpRequest);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            Assert.AreEqual("Ten login został już wybrany. Wybierz inny od existingUser.", ((BadRequestObjectResult)result).Value);
        }

       
    }
}
