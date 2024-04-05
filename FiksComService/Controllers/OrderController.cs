using FiksComService.Application.Infrastructure;
using FiksComService.Models.Database;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FiksComService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class OrderController(
        IOrderRepository orderRepository,
        IOrderDetailRepository orderDetailRepository,
        UserManager<User> userManager,
        ILogger<OrderController> logger
        ) : ControllerBase
    {
        [HttpPost("[action]")]
        public async Task<IActionResult> PlaceOrder()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return BadRequest("Coś poszło nie tak... Spróbuj ponownie później.");
            }

            var cartItems = CartManager.GetItems(HttpContext.Session);
            var order = new Order()
            {
                UserId = user.Id,
                User = user,
                OrderDate = DateTime.UtcNow,
                TotalPrice = cartItems.Sum(item => item.Value * item.Quantity),
                Status = "placed"
            };

            logger.LogInformation(JsonConvert.SerializeObject(order));

            var result = orderRepository.UpsertOrder(order, user);
            if (result > 0)
            {
                //var orderDetails = cartItems.Select(item => new OrderDetail()
                //{
                //    Order = newOrder,
                //    Component = item.Component,
                //    Quantity = item.Quantity,
                //    PricePerUnit = item.Component.Price,
                //});

                //result = orderDetailRepository.UpsertOrderDetails(orderDetails);

                //if (result > 0)
                //{
                //    return Ok("Zamówienie złożono pomyślnie");
                //}

                //return BadRequest("Nie udało się dodać szczegółów zamówienia :(");
                return Ok("Zamówienie złożono pomyślnie");
            }

            return BadRequest("Nie udało się złożyć zamówienia :(");
        }
    }
}
