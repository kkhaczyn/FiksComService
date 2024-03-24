using FiksComService.Models.Database;

namespace FiksComService.Models.Requests
{
    public class AddingNewComponentRequest
    {
        public Component Component { get; set; }

        public IFormFile? Image { get; set; }
    }
}
