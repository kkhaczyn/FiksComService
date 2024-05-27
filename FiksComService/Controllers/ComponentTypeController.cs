using FiksComService.Models.Database;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace FiksComService.Controllers
{
    [EnableCors("default")]
    [Route("api/[controller]")]
    [ApiController]
    public class ComponentTypeController(
        IComponentTypeRepository componentTypeRepository,
        ILogger<ComponentTypeController> logger) 
        : Controller
    {
        //http://localhost:5046/api/componenttype/gettypes
        [Authorize(Roles = "Administrator, Client")]
        [HttpGet("[action]")]
        public IActionResult GetTypes()
        {
            return Ok(componentTypeRepository.GetComponentTypes());
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("[action]")]
        public IActionResult InsertComponentType(ComponentType componentType)
        {
            if (componentType != null)
            {
                var result = componentTypeRepository.InsertComponentType(componentType);

                if (result > 0)
                {
                    logger.LogInformation("Successfuly insert component type.");
                    return Ok("Operacja została zakończona sukcesem.");
                }
            }

            return BadRequest("Coś poszło nie tak... Spróbuj ponownie później.");
        }
    }
}
