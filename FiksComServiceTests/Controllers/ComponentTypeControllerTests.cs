using FiksComService.Controllers;
using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace FiksComServiceTests.Controllers
{
    [TestFixture]
    public class ComponentTypeControllerTests
    {
        private Mock<IComponentTypeRepository> componentTypeRepository;
        private ComponentTypeController componentTypeController;

        [SetUp]
        public void Setup()
        {
            componentTypeRepository = new Mock<IComponentTypeRepository>();
            componentTypeController = new ComponentTypeController(
                componentTypeRepository.Object,
                new Mock<ILogger<ComponentTypeController>>().Object);
        }

        [TearDown]
        public void TearDown()
        {
            componentTypeController.Dispose();
        }

        [Test]
        public void InsertComponentType_WhenAddingNewComponentType_ShouldReturnOKStatusCode()
        {
            // ARRANGE
            ComponentType addingNewComponentTypeRequest = new()
            {
                Code = "SSD",
                Name = "Dysk SSD"
            };

            componentTypeRepository.Setup(x => x.InsertComponentType(It.IsAny<ComponentType>())).Returns(1);

            // ACT
            IActionResult result = componentTypeController.InsertComponentType(addingNewComponentTypeRequest);
            var okResult = result as OkObjectResult;

            // ASSERT
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
        }
    }
}
