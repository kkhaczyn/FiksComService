using FiksComService.Controllers;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FiksComServiceTests.Controllers
{
    [TestFixture]
    public class CartControllerTests
    {
        private Mock<IComponentRepository> _componentRepositoryMock;
        private Mock<ISession> _sessionMock;
        private Mock<HttpContext> _httpContextMock;
        private CartController _controller;

        [SetUp]
        public void Setup()
        {
            _componentRepositoryMock = new Mock<IComponentRepository>();
            _sessionMock = new Mock<ISession>();
            _httpContextMock = new Mock<HttpContext>();
            _httpContextMock.SetupGet(x => x.Session).Returns(_sessionMock.Object);

            _controller = new CartController(_componentRepositoryMock.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = _httpContextMock.Object
                }
            };
        }

        [Test]
        public void Buy_ShouldReturnOkWhenItemAdded()
        {
            // Arrange
            _componentRepositoryMock.Setup(x => x.GetComponentById(It.IsAny<int>()))
                .Returns(new FiksComService.Models.Database.Component()
                {
                    ComponentId = 1,
                    ComponentType = new()
                    {
                        Code = "RAM",
                        Name = "Pamięć RAM"
                    },
                    Manufacturer = "Test",
                    Model = "Test",
                    Price = 10,
                    QuantityAvailable = 10
                });

            // Act
            var result = _controller.Buy(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual("Dodano do koszyka.", okResult.Value);
        }
    }
}
