using FiksComService.Models.Database;

namespace FiksComService.Repositories
{
    public interface IComponentRepository
    {
        IEnumerable<Component> GetComponentsByType(string componentType);
        Component? GetComponentById(int componentId);
        int UpsertComponent(Component component);
    }
}
