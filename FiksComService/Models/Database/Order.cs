﻿namespace FiksComService.Models.Database
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public required string Status { get; set; }
        // TODO: add list of OrderDetail
    }
}
