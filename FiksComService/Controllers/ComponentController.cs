using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FiksComService.Controllers
{
    [EnableCors("default")]
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentController(
        IComponentRepository componentRepository,
        IWebHostEnvironment webHostEnviroment,
        ILogger<ComponentController> logger)
        : ControllerBase
    {
        [Authorize(Roles = "Administrator")]
        [HttpPost("[action]")]
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

        //http://localhost:5046/api/component/getcomponentsbytype/Procesor
        [Authorize(Roles = "Administrator, Client")]
        [HttpGet("[action]/{componentType?}")]
        public IActionResult GetComponentsByType(string? componentType)
        {
            IEnumerable<Component> components = componentRepository.GetComponentsByType(componentType);

            if (components?.Any() ?? false)
            {
                return Ok(components);
            }
            else
            {
                return BadRequest("Brak elementów.");
            }
        }

        //http://localhost:5046/api/component/getcomponentbyid/1
        [Authorize(Roles = "Administrator")]
        [HttpGet("[action]/{componentId:int}")]
        public IActionResult GetComponentById(int componentId)
        {
            Component? component = componentRepository.GetComponentById(componentId);

            if (component != null)
            {
                return Ok(component);
            }

            return BadRequest("Coś poszło nie tak... Spróbuj ponownie później.");
        }

        [Authorize(Roles = "Administrator, Client")]
        [HttpGet("[action]/{componentId:int}")]
        public IActionResult GetComponentImage(int componentId)
        {
            Component? component = componentRepository.GetComponentById(componentId);

            if (component == null)
            {
                return BadRequest("Nie znaleziono komponentu o podanym ID");
            }

            var filePath = Path.Combine(webHostEnviroment.WebRootPath, "ComponentsImages");
            var uniquePosterName = component.Image;

            if (uniquePosterName == null)
            {
                return BadRequest("Nie znaleziono ścieżki do pliku graficznego");
            }

            logger.Log(LogLevel.Information, uniquePosterName);

            var picFilePath = Path.Combine(filePath, uniquePosterName);

            if (!System.IO.File.Exists(picFilePath))
            {
                return BadRequest("Plik graficzny pod podaną ścieżką nie istnieje");
            }

            logger.Log(LogLevel.Information, picFilePath);

            var imageBytes = System.IO.File.ReadAllBytes(picFilePath);

            return File(imageBytes, "image/jpg", uniquePosterName);
        }
    }
}
