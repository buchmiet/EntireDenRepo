using System.Globalization;
using System.Text;
using DataServicesNET80.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace denSharedLibrary;

public class OrdersSummaryToXpsStream : IOrdersSummaryToXpsStream
{
    public class SummaryDocument(SummaryModel model) : IDocument
    {
        public SummaryModel Model { get; } = model;

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container

                .Page(page =>
                {

                    const float horizontalMargin = 0.6f;
                    const float verticalMargin = 0.4f;
                    page.MarginVertical(10);
                    page.MarginHorizontal(horizontalMargin, Unit.Inch);
                    page.DefaultTextStyle(x => x.FontFamily("Courier New"));
                    page.Size(PageSizes.A4.Portrait());                      
                    page.Content().Element(ComposeContent);                      
                });
        }



        void ComposeTable(QuestPDF.Infrastructure.IContainer container)
        {
            container.Table(table =>
            {

                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(20);
                    columns.ConstantColumn(120);
                    columns.ConstantColumn(80);
                    columns.RelativeColumn();
                    columns.ConstantColumn(30);
                    columns.ConstantColumn(30);
                    columns.ConstantColumn(80);

                });


                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#").FontSize(9);
                    header.Cell().Element(CellStyle).Text("Order Id").FontSize(9);
                    header.Cell().Element(CellStyle).Text("Market").FontSize(9);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Product name").FontSize(9);
                    header.Cell().Element(CellStyle).AlignRight().Text("Quantity").FontSize(9);
                    header.Cell().Element(CellStyle).AlignRight().Text("Total").FontSize(9);
                    header.Cell().Element(CellStyle).AlignRight().Text("Location").FontSize(9);
                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.Bold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                int i = 0;
                foreach (var order in Model.SummaryOrderItem)
                {
                    foreach (var item in order.summaryOrderProduct)
                    {
                        table.Cell().Element(cell => CellStyle(cell, i)).Text(Model.SummaryOrderItem.IndexOf(order) + 1).FontSize(11);
                        table.Cell().Element(cell => CellStyle(cell, i)).Text(order.orderTxn).FontSize(9);
                        table.Cell().Element(cell => CellStyle(cell, i)).Text(order.market).FontSize(9);
                        table.Cell().Element(cell => CellStyle(cell, i)).Column(column => {
                            column.Item().Text(Model.ItemNames[item.itembodyID]).FontSize(9);
                            column.Item().Text(Model.ItemMpns[item.itembodyID]).FontSize(7);
                        });
                             
                        table.Cell().Element(cell => CellStyle(cell, i)).Text(item.quantity.ToString()).FontSize(12);
                        table.Cell().Element(cell => CellStyle(cell, i)).Text(Model.TotalQuantities[item.itembodyID]).FontSize(12);
                        table.Cell().Element(cell => CellStyle(cell, i)).Text(Model.ItemLocations[item.itembodyID]).Bold().FontSize(11);
                        i++;
                    }
                    static IContainer CellStyle(IContainer container, int rowIndex)
                    {
                        string colour = rowIndex % 2 == 1 ? "#DDDDDD" : "#FFFFFF";

                        return container.Background(colour).PaddingVertical(1);
                    }

                }
                table.Footer(footer =>
                {
                    footer.Cell().BorderBottom(1).PaddingVertical(5).BorderColor(Colors.Black);
                    footer.Cell().BorderBottom(1).PaddingVertical(5).BorderColor(Colors.Black);
                    footer.Cell().BorderBottom(1).PaddingVertical(5).BorderColor(Colors.Black);
                    footer.Cell().BorderBottom(1).PaddingVertical(5).BorderColor(Colors.Black);
                    footer.Cell().BorderBottom(1).PaddingVertical(5).BorderColor(Colors.Black);
                    footer.Cell().BorderBottom(1).PaddingVertical(5).BorderColor(Colors.Black);
                    footer.Cell().BorderBottom(1).PaddingVertical(5).BorderColor(Colors.Black);
                });
            });
        }

        void ComposeContent(QuestPDF.Infrastructure.IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Item().Column(column =>
                {
                    column.Item().AlignLeft().Text(DateTime.Now.ToString(CultureInfo.CurrentCulture));
                });
                column.Item().Element(ComposeTable);

                column.Spacing(5);
                column.Item().Column(column =>
                {
                    column.Item().AlignLeft().Text("Tools:" + Model.Tools);
                });
                foreach(var item in Model.MultipleOrderSummary) 
                {
                    column.Item().AlignLeft().Text(item);
                }
            });

        }
    }
     
    public async static Task<SummaryModel> GetSummaryModel(List<Complete> Orders, SummaryPrintoutDataPack summaryPrintoutDataPack)
    {
         
        var zwrotka = new SummaryModel();
        var customerOrder = new Dictionary<int,List< int>>();
        foreach (var lis in Orders)
        {
            if (!customerOrder.TryAdd(lis.Customer.customerID, [lis.Order.orderID]))
            {
                customerOrder[lis.Customer.customerID].Add(lis.Order.orderID);
            }

            var itemka = new SummaryOrderItem
            {
                OrderId = lis.Order.orderID,
                orderTxn = summaryPrintoutDataPack.OrderIdsPlatformTXNs[lis.Order.orderID],
                market = summaryPrintoutDataPack.Markety[lis.Order.market]
            };
            foreach (var itemeczka in lis.OrderItems)
            {
                if (!itemeczka.itemName.Equals("Tool"))
                {
                    if (zwrotka.TotalQuantities.ContainsKey(itemeczka.itembodyID))
                    {
                        zwrotka.TotalQuantities[itemeczka.itembodyID] += summaryPrintoutDataPack.OrderItemQuantitiesSold[lis.Order.orderID][itemeczka.itembodyID];
                    } else
                    {
                        zwrotka.TotalQuantities.Add(itemeczka.itembodyID, summaryPrintoutDataPack.OrderItemQuantitiesSold[lis.Order.orderID][itemeczka.itembodyID]);
                    }


                    var przedmiot = new SummaryOrderProduct
                    {
                        itembodyID = itemeczka.itembodyID,
                        quantity = summaryPrintoutDataPack.OrderItemQuantitiesSold[lis.Order.orderID][itemeczka.itembodyID]// itemeczka.quantity* summaryPrintoutDataPack.OrderItemQuantitiesSold[lis.order.orderID][itemeczka.itembodyID],
                    };
                    //if (!zwrotka.totalQuantities.ContainsKey(itemeczka.itembodyID))
                    //{
                    //    zwrotka.totalQuantities[itemeczka.itembodyID] = 0;
                    //}
                    //zwrotka.totalQuantities[itemeczka.itembodyID] = +przedmiot.quantity;
                    zwrotka.ItemNames[itemeczka.itembodyID] = summaryPrintoutDataPack.items[itemeczka.itembodyID].itembody.myname;
                       

                        
                        
                    if (summaryPrintoutDataPack.SoldWiths.TryGetValue(lis.Order.orderID, out var kvp)&&kvp.TryGetValue(itemeczka.itembodyID, out var soldwithId))
                    {
                        zwrotka.ItemMpns[itemeczka.itembodyID] = summaryPrintoutDataPack.items[itemeczka.itembodyID].itembody.mpn+ ", " + summaryPrintoutDataPack.items[soldwithId].itembody.mpn;
                    } else
                    {
                        zwrotka.ItemMpns[itemeczka.itembodyID] = summaryPrintoutDataPack.items[itemeczka.itembodyID].itembody.mpn;
                    }

                    bodyinthebox bb = summaryPrintoutDataPack.items[itemeczka.itembodyID].bodyinthebox;
                    if (bb == null)
                    {
                        zwrotka.ItemLocations[itemeczka.itembodyID] = "";
                    }
                    else
                    {
                        var md = summaryPrintoutDataPack.MultiDrawer.FirstOrDefault(p => p.MultiDrawerID == bb.MultiDrawerID);
                        if (md != null)
                        {
                            zwrotka.ItemLocations[itemeczka.itembodyID] = md.name + '[' + (char)(65 + bb.column) + ',' + (bb.row + 1) + ']';
                        }
                    }
                    itemka.summaryOrderProduct.Add(przedmiot);
                }
                else
                {
                    zwrotka.Tools++;
                }
            }
            zwrotka.SummaryOrderItem.Add(itemka);

        }
        zwrotka.MultipleOrderSummary=new List<string>();
        foreach (var cust in customerOrder.Values.Where(p => p.Count > 1))
        {
            var orders= Orders.Where(p=> cust.Contains(p.Order.orderID)).ToList();
            StringBuilder sb = new StringBuilder();
            zwrotka.MultipleOrderSummary.Add("customer "+ orders[0].Customer.DisplayName+" has "+orders.Count+ " orders ("+string.Join(",",orders.Select(p=>p.Order.orderID))+")");
        }

        return zwrotka;
    }

    public async Task<MemoryStream> GenerateStream(List<Complete> Komplety, SummaryPrintoutDataPack summaryPrintoutDataPack)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var model = await GetSummaryModel(Komplety, summaryPrintoutDataPack);
        IDocument document = new SummaryDocument(model);

        MemoryStream myStream = new();
        document.GenerateXps(myStream);
        return myStream;
    }
}