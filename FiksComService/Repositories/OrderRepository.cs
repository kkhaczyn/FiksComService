using FiksComService.Application;
using FiksComService.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace FiksComService.Repositories
{
    public class OrderRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : IOrderRepository
    {
        public int UpsertOrder(Order order, User user)
        {
            if (order == null || user == null)
            {
                return 0;
            }

            using (var factory = dbContextFactory.CreateDbContext())
            {
                if (order.OrderId == 0)
                {
                    user.Orders.Add(order);
                }
                else
                {
                    var foundOrder = user.Orders.Where(o => o.OrderId == order.OrderId).First();
                    if (foundOrder != null)
                    {
                        foundOrder.Status = order.Status;
                    }
                }

                return factory.SaveChanges();
            }
        }
    }
}
