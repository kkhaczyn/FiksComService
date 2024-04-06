namespace FiksComService.Models.Database
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public required DateTime IssueDate { get; set; }
        public required DateTime DueDate { get; set; }
        public int OrderId { get; set; }
        public string? DocumentGuid { get; set; }
    }
}
