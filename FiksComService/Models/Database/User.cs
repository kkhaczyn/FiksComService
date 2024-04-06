using Microsoft.AspNetCore.Identity;

namespace FiksComService.Models.Database
{
    public class User : IdentityUser<int>
    {
        public virtual ICollection<Order> Orders { get; } = new List<Order>();
    }
}
