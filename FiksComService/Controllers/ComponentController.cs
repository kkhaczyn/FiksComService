using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiksComService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentController(
        IComponentRepository componentRepository,
        IWebHostEnvironment webHostEnviroment,
        ILogger<ComponentController> logger)
        : ControllerBase
    {
        [Authorize(Roles = "Administrator")]
        [HttpPost("/Component/UpsertComponent")]
        public IActionResult UpsertComponent(AddingNewComponentRequest addingNewComponentRequest)
        {
            if (addingNewComponentRequest.Component != null)
            {
                if (addingNewComponentRequest.Image != null)
                {
                    var filePath = Path.Combine(webHostEnviroment.WebRootPath, "ComponentsImages");
                    var uniqePosterName = Guid.NewGuid() + "_" + addingNewComponentRequest.Image.FileName;
                    var picFilePath = Path.Combine(filePath, uniqePosterName);

                    addingNewComponentRequest.Image.CopyTo(new FileStream(picFilePath, FileMode.Create));

                    addingNewComponentRequest.Component.Image = uniqePosterName;
                }

                var result = componentRepository.UpsertComponent(addingNewComponentRequest.Component);

                if (result > 0)
                {
                    logger.LogInformation("Successfuly upsert component.");
                    return Ok("Operacja została zakończona sukcesem.");
                }
            }

            return BadRequest("Coś poszło nie tak... Spróbuj ponownie później.");
        }

        [Authorize(Roles = "Administrator")]
        [Authorize(Roles = "Client")]
        [HttpGet("/Component/GetComponentsByType")]
        public IActionResult GetComponentsByType(string componentType)
        {
            if (!string.IsNullOrWhiteSpace(componentType))
            {
                return Ok(componentRepository.GetComponentsByType(componentType));
            }

            return BadRequest("Coś poszło nie tak... Spróbuj ponownie później.");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("/Component/GetComponentById")]
        public IActionResult GetComponentById(int componentId)
        {
            Component? component = componentRepository.GetComponentById(componentId);

            if (component != null)
            {
                return Ok(component);
            }

            return BadRequest("Coś poszło nie tak... Spróbuj ponownie później.");
        }
    }
}
