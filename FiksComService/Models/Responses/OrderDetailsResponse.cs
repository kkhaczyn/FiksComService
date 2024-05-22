using FiksComService.Models.Database;

namespace FiksComService.Models.Responses
{
    public class OrderDetailsResponse
    {
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public string? InvoiceGuid { get; set; }
    }
}
