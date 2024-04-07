using FiksComService.Models.Database;

namespace FiksComService.Repositories
{
    public interface IInvoiceRepository
    {
        int UpsertInvoice(Invoice invoice);
        Invoice FindByGuid(string guid);
    }
}
