using FiksComService.Application.Infrastructure;
using FiksComService.Models.Cart;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FiksComService.Controllers
{
    //[EnableCors("default")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Client")]
    public class CartController(
        IComponentRepository componentRepository) 
        : ControllerBase
    {
        [HttpGet("[action]")]
        public IActionResult GetCart()
        {
            return Ok(CartManager.GetItems(HttpContext.Session));
        }

        [HttpPut("[action]/{componentId}")]
        public IActionResult Buy(int componentId)
        {
            CartManager.AddToCart(HttpContext.Session, componentRepository, componentId);

            return Ok("Dodano do koszyka.");
        }

        [HttpDelete("[action]/{componentId}")]
        public IActionResult Remove(int componentId)
        {
            var model = new RemoveViewModel()
            {
                ItemId = componentId,
                ItemQuantity = CartManager.RemoveFromCart(HttpContext.Session, componentId),
                CartValue = CartManager.GetCartValue(HttpContext.Session),
                CartQuantityTotal = CartManager.GetCartQuantity(HttpContext.Session)
            };

            return Ok(JsonConvert.SerializeObject(model));
        }
    }
}
