using FiksComService.Controllers;
using FiksComService.Models.Cart;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace FiksComService.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public async Task PlaceOrder_ReturnsBadRequest_WhenItemsNotAvailable()
        {
            // Arrange
            var orderRepositoryMock = new Mock<IOrderRepository>();
            var orderDetailRepositoryMock = new Mock<IOrderDetailRepository>();
            var componentRepositoryMock = new Mock<IComponentRepository>();
            var invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            var userManagerMock = new Mock<UserManager<User>>();
            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();

            var controller = new OrderController(orderRepositoryMock.Object, orderDetailRepositoryMock.Object,
                                                componentRepositoryMock.Object, invoiceRepositoryMock.Object,
                                                webHostEnvironmentMock.Object, userManagerMock.Object);

            var cartItems = new List<CartItem>
            {
                new CartItem { Component = new Component { QuantityAvailable = 0 }, Quantity = 1 }
            };

            // Act
            var result = await controller.PlaceOrder();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }



        private IHttpContextAccessor GetHttpContextAccessor()
        {
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContextMock = new Mock<HttpContext>();
            httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);
            return httpContextAccessorMock.Object;
        }
    }
}
