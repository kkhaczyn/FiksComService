using FiksComService.Application.InvoiceUtils;
using FiksComService.Models.Database;
using FiksComService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Buffers;

namespace FiksComService.Controllers
{
    //[EnableCors("default")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class InvoiceDocumentController(
        IWebHostEnvironment webHostEnviroment,
        IInvoiceRepository invoiceRepository,
        IOrderRepository orderRepository,
        UserManager<User> userManager) : ControllerBase
    {
        [HttpGet("[action]/{documentGuid}")]
        public async Task<IActionResult> GetInvoiceAsync(string documentGuid)
        {
            // Check if client owns the document
            if (!await ClientOwnsDocumentAsync(documentGuid))
            {
                return BadRequest("Nie jesteś właścicielem dokumentu o podanym id");
            }

            // Get path to document
            var documentPath = InvoiceGenerator.GetDocumentPath(documentGuid, webHostEnviroment.WebRootPath);
            
            // Check if path is valid
            if (!System.IO.File.Exists(documentPath))
            {
                return BadRequest("Faktura o podanym identyfikatorze nie istnieje");
            }
            
            // Read pdf as bytes
            var documentBytes = System.IO.File.ReadAllBytes(documentPath);

            // Send file to client
            return File(documentBytes, "application/pdf", documentGuid + ".pdf");
        }

        private async Task<bool> ClientOwnsDocumentAsync(string documentGuid)
        {
            var invoice = invoiceRepository.FindByGuid(documentGuid);

            if (invoice == null)
            {
                return false;
            }

            var order = orderRepository.FindById(invoice.OrderId);

            if (order == null)
            {
                return false;
            }

            var foundUser = await userManager.FindByIdAsync(order.UserId.ToString());

            if (foundUser == null)
            {
                return false;
            }

            var identity = HttpContext.User.Identity;

            if (identity == null)
            {
                return false;
            }

            var currentUserName = identity.Name;

            return foundUser.UserName == currentUserName;
        }
    }
}
