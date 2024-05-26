using System.ComponentModel.DataAnnotations;

namespace FiksComService.Models.Database
{
    public class ComponentType
    {
        [Key]
        public required string Code { get; set; }
        public required string Name { get; set; }
    }
}
