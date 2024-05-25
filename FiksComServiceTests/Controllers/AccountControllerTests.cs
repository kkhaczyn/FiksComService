using FiksComService.Controllers;
using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiksComService.Tests
{
    [TestFixture]
    public class AccountControllerTests
    {
        private AccountController _accountController;
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<SignInManager<User>> _mockSignInManager;
        private Mock<RoleManager<Role>> _mockRoleManager;
        private Mock<ILogger<AccountController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockUserManager = new Mock<UserManager<User>>(MockBehavior.Strict, null, null, null, null, null, null, null, null, null);
            _mockSignInManager = new Mock<SignInManager<User>>(_mockUserManager.Object, null, null, null, null, null, null);
            _mockRoleManager = new Mock<RoleManager<Role>>(MockBehavior.Loose, new List<IRoleStore<Role>>());
            _mockLogger = new Mock<ILogger<AccountController>>();

            _accountController = new AccountController(_mockUserManager.Object, _mockSignInManager.Object, _mockRoleManager.Object, _mockLogger.Object);
        }

        [Test]
        public async Task SignUp_ValidSignUpRequest_ReturnsOkResult()
        {
            // Arrange
            var signUpRequest = new SignUpRequest
            {
                UserName = "testUser",
                Email = "test@example.com",
                Password = "password",
                PhoneNumber = "123456789"
            };

            _mockUserManager.Setup(m => m.FindByNameAsync(signUpRequest.UserName)).ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.FindByEmailAsync(signUpRequest.Email)).ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.CreateAsync(It.IsAny<User>(), signUpRequest.Password))
                .ReturnsAsync(IdentityResult.Success);
            _mockRoleManager.Setup(m => m.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((Role)null);

            // Act
            var result = await _accountController.SignUp(signUpRequest);

            // Assert
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());
        }

        [Test]
        public async Task SignUp_ExistingUsername_ReturnsBadRequestResult()
        {
            // Arrange
            var signUpRequest = new SignUpRequest
            {
                UserName = "existingUser",
                Email = "test@example.com",
                Password = "password",
                PhoneNumber = "123456789"
            };

            var existingUser = new User { UserName = signUpRequest.UserName };

            _mockUserManager.Setup(m => m.FindByNameAsync(signUpRequest.UserName)).ReturnsAsync(existingUser);

            // Act
            var result = await _accountController.SignUp(signUpRequest);

            // Assert
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());
        }

        [Test]
        public async Task SignUp_ExistingEmail_ReturnsBadRequestResult()
        {
            // Arrange
            var signUpRequest = new SignUpRequest
            {
                UserName = "testUser",
                Email = "existing@example.com",
                Password = "password",
                PhoneNumber = "123456789"
            };

            var existingUser = new User { Email = signUpRequest.Email };

            _mockUserManager.Setup(m => m.FindByNameAsync(signUpRequest.UserName)).ReturnsAsync((User)null);
            _mockUserManager.Setup(m => m.FindByEmailAsync(signUpRequest.Email)).ReturnsAsync(existingUser);

            // Act
            var result = await _accountController.SignUp(signUpRequest);

            // Assert
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());
        }
    }
}
