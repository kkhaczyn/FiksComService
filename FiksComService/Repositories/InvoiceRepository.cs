using FiksComService.Application;
using FiksComService.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace FiksComService.Repositories
{
    public class InvoiceRepository(
        IDbContextFactory<ApplicationContext> dbContextFactory)
        : IInvoiceRepository
    {
        public int UpsertInvoice(Invoice invoice)
        {
            if (invoice == null)
            {
                return 0;
            }

            using (var factory = dbContextFactory.CreateDbContext())
            {
                if (invoice.InvoiceId == 0)
                {
                    factory.Invoices.Add(invoice);
                }
                else
                {
                    factory.Invoices.Update(invoice);
                }

                return factory.SaveChanges();
            }
        }

        public Invoice FindByGuid(string guid)
        {
            using (var factory = dbContextFactory.CreateDbContext())
            {
                var invoice = factory.Invoices.Single(invoice => invoice.DocumentGuid == guid);

                return invoice;
            }
        }
    }
}
