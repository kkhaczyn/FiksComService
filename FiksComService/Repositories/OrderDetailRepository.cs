using FiksComService.Application;
using FiksComService.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace FiksComService.Repositories
{
    public class OrderDetailRepository(IDbContextFactory<ApplicationContext> dbContextFactory) 
        : IOrderDetailRepository
    {
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
