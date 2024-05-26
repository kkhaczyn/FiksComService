using FiksComService.Application;
using FiksComService.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace FiksComService.Repositories
{
    public class ComponentTypeRepository(IDbContextFactory<ApplicationContext> dbContextFactory) : IComponentTypeRepository
    {
        public IEnumerable<ComponentType> GetComponentTypes()
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                return factory.ComponentTypes.ToList();
            }
        }

        public int InsertComponentType(ComponentType componentType)
        {
            if (componentType == null)
                return 0;

            using (var factory = dbContextFactory.CreateDbContext())
            {
                if (factory.ComponentTypes.Any(x 
                    => x.Code.ToUpper() == componentType.Code.ToUpper()
                        || x.Name.ToUpper() == componentType.Name.ToUpper()))
                {
                    return 0;
                }
                componentType.Code = componentType.Code.ToUpper();
                factory.ComponentTypes.Add(componentType);
                return factory.SaveChanges();
            }
        }
    }
}
