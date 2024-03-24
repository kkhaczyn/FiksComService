namespace FiksComService.Models.Cart
{
    public class RemoveViewModel
    {
        public int ItemId { get; set; }

        public int ItemQuantity { get; set; }

        public decimal CartValue { get; set; }

        public int CartQuantityTotal { get; set; }
    }
}
