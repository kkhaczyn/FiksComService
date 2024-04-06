using FiksComService.Application.InvoiceUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Buffers;

namespace FiksComService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Client")]
    public class InvoiceDocumentController(
        IWebHostEnvironment webHostEnviroment) : ControllerBase
    {
        [HttpGet("[action]/{documentGuid}")]
        public IActionResult GetInvoice(string documentGuid)
        {
            // TODO:
            // Check if path is valid
            // Check if given document is associated with user requesting doc
            // Return document as webpage

            // Read pdf as bytes
            var documentPath = InvoiceGenerator.GetDocumentPath(documentGuid, webHostEnviroment.WebRootPath);
            var documentBytes = System.IO.File.ReadAllBytes(documentPath);

            return File(documentBytes, "application/pdf", documentGuid + ".pdf");
        }
    }
}
