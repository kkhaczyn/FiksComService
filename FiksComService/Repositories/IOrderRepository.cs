using FiksComService.Models.Database;

namespace FiksComService.Repositories
{
    public interface IOrderRepository
    {
        int UpsertOrder(Order order);
        Order? FindById(int id);
        List<Order> FindByUserId(int userId);
    }
}
