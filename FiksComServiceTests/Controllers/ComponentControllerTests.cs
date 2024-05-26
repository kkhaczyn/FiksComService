using FiksComService.Controllers;
using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FiksComServiceTests.Controllers
{
    [TestFixture]
    public class ComponentControllerTests
    {
        private Mock<IComponentRepository> componentRepository;
        private Mock<IWebHostEnvironment> webHostEnviroment;
        private ComponentController componentController;

        [SetUp]
        public void Setup()
        {
            componentRepository = new Mock<IComponentRepository>();
            webHostEnviroment = new Mock<IWebHostEnvironment>();
            componentController = new ComponentController(
                componentRepository.Object,
                webHostEnviroment.Object,
                new Mock<ILogger<ComponentController>>().Object);
        }

        [Test]
        public void UpsertComponent_WhenAddingNewComponent_ShouldReturnOKStatusCode()
        {
            // ARRANGE
            AddingNewComponentRequest addingNewComponentRequest = new()
            {
                Component = new()
                {
                    ComponentType = new ComponentType() { Code = "RAM", Name = "Pamięć RAM"},
                    Manufacturer = "xxx",
                    Model = "xxx",
                    Price = 1.34M,
                    QuantityAvailable = 1,
                }
            };

            componentRepository.Setup(x => x.UpsertComponent(It.IsAny<Component>())).Returns(1);

            // ACT
            IActionResult result = componentController.UpsertComponent(addingNewComponentRequest);
            var okResult = result as OkObjectResult;

            // ASSERT
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public void GetComponentsByType_WhenCalledWithTypeProcesor_ShouldReturnOnlyComponentsWithTypeProcesor()
        {
            // ARRANGE
            componentRepository.Setup(x => x.GetComponentsByType(It.IsAny<string>())).Returns(
            [
                new()
                {
                    ComponentType = new ComponentType() { Code = "RAM", Name = "Pamięć RAM"},
                    Manufacturer = "xxx",
                    Model = "xxx",
                    Price = 1.34M,
                    QuantityAvailable = 1,
                }
            ]);

            // ACT
            IActionResult result = componentController.GetComponentsByType("ram");
            var okResult = result as OkObjectResult;

            // ASSERT
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.Not.Null);
        }

        [Test]
        public void GetComponentsById_WhenCalledWithId_ShouldReturnOnlyComponentWithSpecifiedId()
        {
            // ARRANGE
            componentRepository.Setup(x => x.GetComponentById(It.IsAny<int>())).Returns(
                new Component()
                {
                    ComponentId = 1,
                    ComponentType = new ComponentType() { Code = "RAM", Name = "Pamięć RAM" },
                    Manufacturer = "xxx",
                    Model = "xxx",
                    Price = 1.34M,
                    QuantityAvailable = 1,
                });

            // ACT
            IActionResult result = componentController.GetComponentById(1);
            var okResult = result as OkObjectResult;

            // ASSERT
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.Not.Null);
        }
    }
}
