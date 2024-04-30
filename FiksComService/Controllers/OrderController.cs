using FiksComService.Application.Infrastructure;
using FiksComService.Application.InvoiceUtils;
using FiksComService.Models.Cart;
using FiksComService.Models.Database;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FiksComService.Controllers
{
    [EnableCors("default")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class OrderController(
        IOrderRepository orderRepository,
        IComponentRepository componentRepository,
        IInvoiceRepository invoiceRepository,
        IWebHostEnvironment webHostEnviroment,
        UserManager<User> userManager
        ) : ControllerBase
    {
        private InvoiceGenerator invoiceGenerator { get; } = 
            new InvoiceGenerator(invoiceRepository, webHostEnviroment);

        [HttpPost("[action]")]
        public async Task<IActionResult> PlaceOrder()
        {
            var cartItems = CartManager.GetItems(HttpContext.Session);

            if (!CheckIfItemsAreAvailable(cartItems))
            {
                return BadRequest("Liczba elementów w koszyku przekracza liczbę dostępnych komponentów w sklepie");
            }

            var order = await CreateAndAddOrderAsync();

            if (order == null)
            {
                return BadRequest("Nie udało się utworzyć zamówienia :(");
            }

            order.OrderDetails = cartItems.Select(item => new OrderDetail()
            {
                OrderId = order.OrderId,
                Order = order,
                Component = item.Component,
                Quantity = item.Quantity,
                PricePerUnit = item.Component.Price,
            }).ToList();

            var orderUpdateResult = orderRepository.UpsertOrder(order);

            if (orderUpdateResult > 0)
            {
                UpdateComponentsQuantities(order.OrderDetails);

                CartManager.ClearCart(HttpContext.Session);

                var invoicePath = invoiceGenerator.GenerateInvoice(order);

                return Ok($"Zamówienie złożono pomyślnie. Link do faktury: {invoicePath}");
            }

            return BadRequest("Nie udało się dodać szczegółów zamówienia :(");
        }

        private bool CheckIfItemsAreAvailable(List<CartItem> cartItems)
        {
            foreach (var cartItem in cartItems)
            {
                var component = cartItem.Component;
                if (component.QuantityAvailable - cartItem.Quantity < 0)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<Order?> CreateAndAddOrderAsync()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return null;
            }

            var order = new Order()
            {
                UserId = user.Id,
                User = user,
                OrderDate = DateTime.UtcNow,
                TotalPrice = CartManager.GetCartValue(HttpContext.Session),
                Status = "placed"
            };

            user.Orders.Add(order);

            var result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return order;
            }

            return null;
        }

        private void UpdateComponentsQuantities(ICollection<OrderDetail> orderDetails)
        {
            foreach (var orderDetail in orderDetails)
            {
                var component = orderDetail.Component;
                component.QuantityAvailable -= orderDetail.Quantity;
                componentRepository.UpsertComponent(component);
            }
        }
    }
}
