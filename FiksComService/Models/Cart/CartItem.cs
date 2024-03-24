using FiksComService.Models.Database;

namespace FiksComService.Models.Cart
{
    public class CartItem
    {
        public Component Component { get; set; }

        public int Quantity { get; set; }

        public decimal Value { get; set; }
    }
}
