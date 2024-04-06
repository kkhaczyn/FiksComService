using FiksComService.Models.Database;

namespace FiksComService.Repositories
{
    public interface IOrderRepository
    {
        int UpsertOrder(Order order);
    }
}
