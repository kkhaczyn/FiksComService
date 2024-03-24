using FiksComService.Application;
using FiksComService.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace FiksComService.Repositories
{
    public class ComponentRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : IComponentRepository
    {
        public Component? GetComponentById(int componentId)
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                return factory.Components.FirstOrDefault(x => x.ComponentId == componentId);
            }
        }

        public IEnumerable<Component> GetComponentsByType(string componentType)
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                return factory.Components.Where(x => x.ComponentType.ToUpper() == componentType.ToUpper());
            }
        }

        public int UpsertComponent(Component component)
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                if (component?.ComponentId != null)
                {
                    factory.Components.Add(component);
                }
                else
                {
                    factory.Components.Update(component);
                }

                return factory.SaveChanges();
            }
        }
    }
}
