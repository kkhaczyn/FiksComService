using FiksComService.Controllers;
using FiksComService.Models.Database;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace FiksComServiceTests.Controllers
{
    [TestFixture]
    public class OrderControllerTests
    {
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IOrderDetailRepository> _orderDetailRepositoryMock;
        private Mock<IComponentRepository> _componentRepositoryMock;
        private Mock<IInvoiceRepository> _invoiceRepositoryMock;
        private Mock<IWebHostEnvironment> _webHostEnvironmentMock;
        private Mock<UserManager<User>> _userManagerMock;
        private Mock<HttpContext> _httpContextMock;
        private OrderController _controller;

        [SetUp]
        public void Setup()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _orderDetailRepositoryMock = new Mock<IOrderDetailRepository>();
            _componentRepositoryMock = new Mock<IComponentRepository>();
            _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            _webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _httpContextMock = new Mock<HttpContext>();

            _controller = new OrderController(
                _orderRepositoryMock.Object,
                _orderDetailRepositoryMock.Object,
                _componentRepositoryMock.Object,
                _invoiceRepositoryMock.Object,
                _webHostEnvironmentMock.Object,
                _userManagerMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _httpContextMock.Object
                }
            };
        }

        [Test]
        public async Task GetMyOrders_ShouldReturnUserOrders_WhenUserIsAuthenticated()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "testuser" };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, user.UserName) }, "TestAuthType"));
            _httpContextMock.Setup(x => x.User).Returns(claimsPrincipal);

            _userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);
            _orderRepositoryMock.Setup(x => x.FindByUserId(user.Id)).Returns(new List<Order>() { new Order() { Status = "Placed"} });

            // Act
            var result = await _controller.GetMyOrders();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<IEnumerable<Order>>(okResult.Value);
        }

        [Test]
        public async Task GetUserOrders_ShouldReturnUserOrders_WhenUserIsFound()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, UserName = "testuser" };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync(user);
            _orderRepositoryMock.Setup(x => x.FindByUserId(userId)).Returns(new List<Order>() { new Order() { Status = "Placed" } });

            // Act
            var result = await _controller.GetUserOrders(userId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsInstanceOf<IEnumerable<Order>>(okResult.Value);
        }

        [Test]
        public async Task GetUserOrders_ShouldReturnNotFound_WhenUserIsNotFound()
        {
            // Arrange
            var userId = 1;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUserOrders(userId);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }
    }
}
