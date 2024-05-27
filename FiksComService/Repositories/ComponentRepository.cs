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
                return factory.Components
                    .Include(c => c.ComponentType)
                    .FirstOrDefault(x => x.ComponentId == componentId);
            }
        }

        public IEnumerable<Component> GetComponentsByType(string? componentType)
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                if (string.IsNullOrWhiteSpace(componentType))
                {
                    return factory.Components
                        .Include(c => c.ComponentType)
                        .ToList();
                }


                return factory.Components
                    .Include(c => c.ComponentType)
                    .Where(x => x.ComponentType.Name.ToUpper() == componentType.ToUpper()
                    || x.ComponentType.Code.ToUpper() == componentType.ToUpper())
                    .ToList();
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
                    factory.ComponentTypes.Attach(component.ComponentType);
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
