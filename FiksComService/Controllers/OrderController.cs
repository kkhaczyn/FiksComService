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
        UserManager<User> userManager
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

            user.Orders.Add(order);

            var userUpdateResult = await userManager.UpdateAsync(user);

            if (userUpdateResult.Succeeded)
            {
                var orderDetails = cartItems.Select(item => new OrderDetail()
                {
                    OrderId = order.OrderId,
                    Order = order,
                    Component = item.Component,
                    Quantity = item.Quantity,
                    PricePerUnit = item.Component.Price,
                }).ToList();

                order.OrderDetails = orderDetails;

                var orderUpdateResult = orderRepository.UpsertOrder(order);

                if (orderUpdateResult > 0)
                {
                    return Ok("Zamówienie złożono pomyślnie");
                }

                return BadRequest("Nie udało się dodać szczegółów zamówienia :(");
            }

            return BadRequest("Nie udało się złożyć zamówienia :(");
        }
    }
}
