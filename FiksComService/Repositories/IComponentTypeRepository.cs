using FiksComService.Models.Database;

namespace FiksComService.Repositories
{
    public interface IComponentTypeRepository
    {
        IEnumerable<ComponentType> GetComponentTypes();
        int InsertComponentType(ComponentType componentType);
    }
}
