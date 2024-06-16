using FiksComService.Application.Infrastructure;
using FiksComService.Application.InvoiceUtils;
using FiksComService.Models.Cart;
using FiksComService.Models.Database;
using FiksComService.Models.Requests;
using FiksComService.Models.Responses;
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
    public class OrderController(
        IOrderRepository orderRepository,
        IOrderDetailRepository orderDetailRepository,
        IComponentRepository componentRepository,
        IInvoiceRepository invoiceRepository,
        IWebHostEnvironment webHostEnviroment,
        UserManager<User> userManager
        ) : ControllerBase
    {
        private InvoiceGenerator invoiceGenerator { get; } = 
            new InvoiceGenerator(invoiceRepository, webHostEnviroment);

        [Authorize(Roles = "Client")]
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
                ComponentId = item.Component.ComponentId,
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
                Status = "opłacone"
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

        [Authorize(Roles = "Client")]
        [HttpGet("[action]")]
        public async Task<IActionResult> GetMyOrders()
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            return GetUserOrders(user);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet("[action]/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            return GetUserOrders(user);
        }

        private IActionResult GetUserOrders(User? user)
        {
            if (user == null)
            {
                return BadRequest("Nie znaleziono użytkownika o podanym ID");
            }

            var orders = orderRepository.FindByUserId(user.Id);

            return Ok(orders);
        }

        [Authorize(Roles = "Client, Administrator")]
        [HttpGet("[action]/{orderId}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var user = await userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return BadRequest("Błąd użytkownika");
            }

            if (HttpContext.User.IsInRole("Client"))
            {
                var order = orderRepository.FindById(orderId);

                if (order != null && order.UserId != user.Id)
                {
                    return BadRequest("Użytkownik nie posiada uprawnień do tego zasobu");
                }
            }

            var invoice = invoiceRepository.FindByOrderId(orderId);
            var orderDetails = orderDetailRepository
                .GetOrderDetailsByOrderId(orderId)
                .Select(orderDetail => {
                    var component = componentRepository.GetComponentById(orderDetail.ComponentId);

                    if (component != null)
                    {
                        orderDetail.Component = component;
                    }
                    
                    return orderDetail;
                }).ToList();

            var orderDetailsResponse = new OrderDetailsResponse
            {
                OrderDetails = orderDetails,
                InvoiceGuid = invoice.DocumentGuid
            };

            return Ok(orderDetailsResponse);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("[action]")]
        public async Task<IActionResult> ChangeOrderStatus(ChangeOrderStatusRequest changeOrderStatusRequest)
        {
            string[] allowedStatuses = ["opłacone", "dostarczone", "anulowane"];

            if (!allowedStatuses.Contains(changeOrderStatusRequest.OrderStatus.ToLower()))
            {
                return BadRequest("Niedozwolony status zamówienia");
            }

            var order = orderRepository.FindById(changeOrderStatusRequest.OrderId);

            if (order == null)
            {
                return BadRequest("Nie znaleziono zamówienia o podanym ID");
            }

            order.Status = changeOrderStatusRequest.OrderStatus;

            if (orderRepository.UpsertOrder(order) <= 0)
            {
                return BadRequest("Nie udało się zaktualizować statusu zamówienia");
            }

            return Ok("Status zamówienia zmieniony pomyślnie");
        }
    }
}
