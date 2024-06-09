using FiksComService.Models.Database;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FiksComService.Application.InvoiceUtils
{
    public class InvoiceDocument : IDocument
    {
        public Invoice Model { get; }
        private List<OrderDetail> OrderDetails;
        private string? UserInformation;

        public InvoiceDocument(
            Invoice invoice, List<OrderDetail> orderDetails, string? userInformation)
        {
            Model = invoice;
            OrderDetails = orderDetails;
            UserInformation = userInformation;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);


                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
        }

        void ComposeHeader(IContainer container)
        {
            var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item().Text($"Faktura #{Model.InvoiceId}").Style(titleStyle);

                    column.Item().Text(text =>
                    {
                        text.Span("Data wystawienia: ").SemiBold();
                        text.Span($"{Model.IssueDate:d}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Termin zapłaty: ").SemiBold();
                        text.Span($"{Model.DueDate:d}");
                    });

                    column.Item().Text(text =>
                    {
                        text.Span("Dane osobowe: ").SemiBold();
                        text.Span($"{UserInformation}");
                    });
                });

                row.ConstantItem(100).Height(50).Placeholder();
            });
        }

        void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(ComposeTable);
            });
        }

        void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                // step 1
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                // step 2
                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Komponent");
                    header.Cell().Element(CellStyle).AlignRight().Text("Cena jednostkowa");
                    header.Cell().Element(CellStyle).AlignRight().Text("Ilość");
                    header.Cell().Element(CellStyle).AlignRight().Text("Cena całkowita");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                // step 3
                var i = 0;
                foreach (var item in OrderDetails)
                {
                    table.Cell().Element(CellStyle).Text((++i).ToString());
                    table.Cell().Element(CellStyle).Text(item.Component.Model);
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Component.Price.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                    table.Cell().Element(CellStyle).AlignRight().Text($"{item.Quantity * item.PricePerUnit} zł");

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                }
            });
        }
    }
}
