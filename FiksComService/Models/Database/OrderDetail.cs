namespace FiksComService.Models.Database
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public required Order Order {  get; set; }
        public required Component Component {  get; set; }
        public int Quantity {  get; set; }
        public decimal PricePerUnit {  get; set; }
    }
}
