namespace FiksComService.Models.Database
{
    public class Component
    {
        public int ComponentId { get; set; }
        public required ComponentType ComponentType { get; set; }
        public required string Manufacturer { get; set; }
        public required string Model { get; set; }
        public required decimal Price { get; set; }
        public required int QuantityAvailable { get; set; }
        public string? Image { get; set; }
    }
}
