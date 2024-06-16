namespace FiksComService.Models.Requests
{
    public class ChangeOrderStatusRequest
    {
        public required int OrderId { get; set; }
        public required string OrderStatus { get; set; }
    }
}
