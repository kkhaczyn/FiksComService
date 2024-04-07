using FiksComService.Application;
using FiksComService.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace FiksComService.Repositories
{
    public class OrderRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : IOrderRepository
    {
        public int UpsertOrder(Order order)
        {
            if (order == null)
            {
                return 0;
            }

            using (var factory = dbContextFactory.CreateDbContext())
            {
                if (order.OrderId == 0)
                {
                    factory.Orders.Add(order);
                }
                else
                {
                    factory.Orders.Update(order);
                }

                return factory.SaveChanges();
            }
        }
        public Order? FindById(int id)
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                var order = factory.Orders.Find(id);

                return order;
            }
        }
    }
}
