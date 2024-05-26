using FiksComService.Application;
using FiksComService.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace FiksComService.Repositories
{
    public class OrderDetailRepository(IDbContextFactory<ApplicationContext> dbContextFactory) 
        : IOrderDetailRepository
    {
        public List<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                var orderDetails = factory.OrderDetails.Include(x => x.Component)
                    .Include(y=>y.Component.ComponentType)
                    .Where(orderDetail => orderDetail.OrderId ==  orderId)
                    .ToList();

                return orderDetails;
            }
        }

        public int UpsertOrderDetails(IEnumerable<OrderDetail> orderDetails)
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                foreach (var orderDetail in orderDetails)
                {
                    factory.OrderDetails.Add(orderDetail);
                }

                return factory.SaveChanges();
            }
        }
    }
}
