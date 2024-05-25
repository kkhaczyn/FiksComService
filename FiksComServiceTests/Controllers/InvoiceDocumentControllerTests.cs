using FiksComService.Controllers;
using FiksComService.Repositories;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace FiksComService.Tests.Controllers
{
    public class InvoiceDocumentControllerTests
    {
        [Fact]
        public async Task GetInvoiceAsync_ClientOwnsDocument_ReturnsFile()
        {
            // Arrange
            var documentGuid = "12345";

            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            var orderRepositoryMock = new Mock<IOrderRepository>();
            var userManagerMock = new Mock<UserManager<User>>();

            var controller = new InvoiceDocumentController(
                webHostEnvironmentMock.Object,
                invoiceRepositoryMock.Object,
                orderRepositoryMock.Object,
                userManagerMock.Object);

            invoiceRepositoryMock.Setup(repo => repo.FindByGuid(documentGuid))
                                .Returns(new Invoice { OrderId = 1 });

            orderRepositoryMock.Setup(repo => repo.FindById(1))
                                .Returns(new Order { UserId = "123" });

            userManagerMock.Setup(manager => manager.FindByIdAsync("123"))
                                .ReturnsAsync(new User { UserName = "TestUser" });

            // Act
            var result = await controller.GetInvoiceAsync(documentGuid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("application/pdf", result.ContentType);
        }

        [Fact]
        public async Task GetInvoiceAsync_ClientDoesNotOwnDocument_ReturnsBadRequest()
        {
            // Arrange
            var documentGuid = "12345";

            var webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
            var invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            var orderRepositoryMock = new Mock<IOrderRepository>();
            var userManagerMock = new Mock<UserManager<User>>();

            var controller = new InvoiceDocumentController(
                webHostEnvironmentMock.Object,
                invoiceRepositoryMock.Object,
                orderRepositoryMock.Object,
                userManagerMock.Object);

            invoiceRepositoryMock.Setup(repo => repo.FindByGuid(documentGuid))
                                .Returns(new Invoice { OrderId = 1 });

            orderRepositoryMock.Setup(repo => repo.FindById(1))
                                .Returns(new Order { UserId = "456" });

            userManagerMock.Setup(manager => manager.FindByIdAsync("456"))
                                .ReturnsAsync(new User { UserName = "TestUser" });

            // Act
            var result = await controller.GetInvoiceAsync(documentGuid);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("400", result.StatusCode);
        }
    }
}
