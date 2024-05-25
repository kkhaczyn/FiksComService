using FiksComService.Controllers;
using FiksComService.Models.Cart;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using Xunit;

namespace FiksComService.Tests
{
    public class CartControllerTests
    {
        [Fact]
        public void GetCart_ReturnsOkResult()
        {
            // Arrange
            var session = new Mock<ISession>();
            var componentRepository = new Mock<IComponentRepository>();
            CartManager.AddToCart(session.Object, componentRepository.Object, 1);
            var controller = new CartController(componentRepository.Object);

            // Act
            var result = controller.GetCart() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Cart>(result.Value);
        }

        [Fact]
        public void Buy_ReturnsOkResult()
        {
            // Arrange
            var session = new Mock<ISession>();
            var componentRepository = new Mock<IComponentRepository>();
            var controller = new CartController(componentRepository.Object);

            // Act
            var result = controller.Buy(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Dodano do koszyka.", result.Value);
        }

        [Fact]
        public void Remove_ReturnsOkResult()
        {
            // Arrange
            var session = new Mock<ISession>();
            var componentRepository = new Mock<IComponentRepository>();
            CartManager.AddToCart(session.Object, componentRepository.Object, 1);
            var controller = new CartController(componentRepository.Object);

            // Act
            var result = controller.Remove(1) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result.Value);
        }
    }
}
