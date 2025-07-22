using DataServicesNET80.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace denSharedLibrary;

public class OrderItemsToImageByteArray
{
    public class OrderItemsModel
    {
        public string CurrencySymbol;
        public List<orderitem> Items;
    }


    public class SummaryDocument : IDocument
    {
        public OrderItemsModel Model { get; }

        public SummaryDocument(OrderItemsModel model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.MarginVertical(0, Unit.Millimetre);
                    page.MarginHorizontal(0, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Calibri"));
                    page.Size(new PageSize(95, 24, Unit.Millimetre));
                    page.Content().Element(ComposeContent);

                });
        }


        void ComposeContent(QuestPDF.Infrastructure.IContainer container)
        {

            container.Table(table =>
            {

                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(50, Unit.Millimetre);
                    columns.ConstantColumn(15, Unit.Millimetre);
                    columns.ConstantColumn(15, Unit.Millimetre);
                    columns.ConstantColumn(15, Unit.Millimetre);
                });

                int i = 0;
                foreach (var item in Model.Items)
                {
                    int weight = 0;
                    if (item.ItemWeight == null)
                    {
                        weight = 0;
                    }
                    table.Cell().AlignLeft().Text(item.itemName).FontSize(9);
                    table.Cell().AlignCenter().Text(item.quantity.ToString()).FontSize(9);
                    table.Cell().AlignCenter().Text(weight.ToString()).FontSize(9);
                    table.Cell().AlignCenter().Text(Model.CurrencySymbol + item.price.ToString()).FontSize(9);
                    i++;
                }
            });

        }
    }

    public OrderItemsModel GetOrderItemsModelModel(List<orderitem> items, string currencySymbol)
    {
        return new OrderItemsModel
        {
            Items = items,
            CurrencySymbol = currencySymbol

        };
    }


    public byte[] GenerateStream(List<orderitem> items, string currencysymbol)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var model = GetOrderItemsModelModel(items, currencysymbol);
        IDocument document = new SummaryDocument(model);
        return document.GenerateImages().First();
    }
}