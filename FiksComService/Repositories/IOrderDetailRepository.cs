using FiksComService.Models.Database;

namespace FiksComService.Repositories
{
    public interface IOrderDetailRepository
    {
        int UpsertOrderDetails(IEnumerable<OrderDetail> orderDetails);
        List<OrderDetail> GetOrderDetailsByOrderId(int orderId);
    }
}
