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

        public IEnumerable<Component> GetComponentsByType(string? componentType)
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                if (string.IsNullOrWhiteSpace(componentType))
                    return factory.Components.ToList();

                return factory.Components.Where(x => x.ComponentType.ToUpper() == componentType.ToUpper()).ToList();
            }
        }

        public int UpsertComponent(Component component)
        {
            if (component == null)
                return 0;

            using (var factory = dbContextFactory.CreateDbContext())
            {
                if (component.ComponentId == 0)
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
