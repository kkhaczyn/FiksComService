using FiksComService.Models.Database;
using FiksComService.Repositories;
using QuestPDF.Fluent;

namespace FiksComService.Application.InvoiceUtils
{
    public class InvoiceGenerator(
        IInvoiceRepository invoiceRepository,
        IWebHostEnvironment webHostEnviroment)
    {
        private static readonly string InvoicesFolderName = "Invoices";

        // Returns url/path to invoice generated for given order
        public string GenerateInvoice(Order order)
        {
            // Create invoice for order
            var invoice = CreateInvoice(order);

            // Save invoice details in database
            var upsertResult = invoiceRepository.UpsertInvoice(invoice);

            if (upsertResult <= 0)
            {
                return "";
            }

            // Generate PDF file and save it
            var document = new InvoiceDocument(invoice);
            var documentGuid = GeneratePdf(document);

            // Save name of file in invoice record
            invoice.DocumentGuid = documentGuid;
            upsertResult = invoiceRepository.UpsertInvoice(invoice);
            
            // Return path/url to file
            return documentGuid;
        }

        private Invoice CreateInvoice(Order order)
        {
            var issueDate = DateTime.UtcNow;

            return new Invoice
            {
                IssueDate = issueDate,
                DueDate = issueDate + TimeSpan.FromDays(14),
                OrderId = order.OrderId
            };
        }

        private string GeneratePdf(InvoiceDocument document)
        {
            var filePath = Path.Combine(webHostEnviroment.WebRootPath, InvoicesFolderName);
            var guid = Guid.NewGuid().ToString();
            var uniqueDocumentName = guid + "_Invoice.pdf";
            var documentFilePath = Path.Combine(filePath, uniqueDocumentName);

            document.GeneratePdf(documentFilePath);

            return guid;
        }
    }
}
