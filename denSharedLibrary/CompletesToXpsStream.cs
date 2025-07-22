using System.Text;
using DataServicesNET80.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace denSharedLibrary;

public class CompletesToXpsStream: ICompletesToXpsStream
{
    public class InvoiceDocument : IDocument
    {
        public InvoiceModel Model { get; }

        public InvoiceDocument(InvoiceModel model)
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
                    const float horizontalMargin = 0.6f;
                    const float verticalMargin = 0.4f;
                    page.MarginVertical(10);
                    page.MarginHorizontal(horizontalMargin, Unit.Inch);
                    page.DefaultTextStyle(x => x.FontFamily("Courier New"));
                    page.Size(PageSizes.A4.Portrait());

                    page.Header().PaddingTop(5).PaddingVertical(verticalMargin, Unit.Inch).Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Height(70).PaddingBottom(30).AlignCenter().Column(column =>
                    {
                        column.Item().AlignLeft().Text(Model.MarketName);
                    });
                    page.Background()
                        .Decoration(decoration =>
                        {
                            decoration.Before().PaddingTop(15).Element(DrawTopAndBottom);
                            decoration.Content().Extend();
                            decoration.After().PaddingBottom(28).Element(DrawTopAndBottom);
                            void DrawTopAndBottom(QuestPDF.Infrastructure.IContainer container)
                            {
                                container.PaddingHorizontal(5).Column(column =>
                                {
                                    column.Item().
                                        Row(row =>
                                        {
                                            row.RelativeItem().PaddingLeft(16).AlignLeft().Column(column =>
                                            {
                                                column.Item().AlignLeft().Text("TIME4PARTS.CO.UK").FontSize(12).FontColor(Colors.Grey.Medium);
                                                column.Item().AlignLeft().Text("TIME4PARTS.COM").FontSize(12).FontColor(Colors.Grey.Medium);
                                            });
                                            row.AutoItem().AlignCenter().Column(column =>
                                            {
                                                column.Item().AlignCenter().Text("TIME4PARTS.CO.UK").FontSize(12).FontColor(Colors.Grey.Medium);
                                                column.Item().AlignCenter().Text("TIME4PARTS.COM").FontSize(12).FontColor(Colors.Grey.Medium);
                                            });
                                            row.RelativeItem().PaddingRight(26).AlignRight().Column(column =>
                                            {
                                                column.Item().AlignRight().Text("TIME4PARTS.CO.UK").FontSize(12).FontColor(Colors.Grey.Medium);
                                                column.Item().AlignRight().Text("TIME4PARTS.COM").FontSize(12).FontColor(Colors.Grey.Medium);
                                            });

                                        });
                                });
                            }
                        });
                });
        }

        void ComposeHeader(QuestPDF.Infrastructure.IContainer container)
        {
            container.Column(column =>
            {
                column.Item().Row(row =>
                {
                    row.RelativeItem().AlignLeft().Column(column =>
                    {
                        column.Item().Text("Buchmiet Ltd").FontSize(20).Bold();//;
                        column.Item().Text("70/4 Salamander Street").LineHeight(1f).FontSize(11);//;
                        column.Item().Text("Edinburgh").LineHeight(1f).FontSize(11);//;
                        column.Item().Text("EH6 7JY").FontSize(11).LineHeight(1f);//;
                        column.Item().Text("United Kingdom").LineHeight(1f);//.FontSize(11);
                        column.Item().Text("Vat reg no.: 300306082").LineHeight(1f).FontSize(11);//;
                    });
                    row.RelativeItem();
                    row.RelativeItem().AlignRight().Column(column =>
                    {
                        column.Item().AlignRight().Text("VAT INVOICE").FontSize(22);
                        column.Item().
                            Row(row =>
                            {
                                row.RelativeItem().AlignLeft().Column(column =>
                                {
                                    column.Item().Text("Invoice no.").FontSize(11);
                                });
                                row.RelativeItem().AlignRight().Column(column =>
                                {
                                    column.Item().Text(Model.InvoiceNumber).FontSize(11);
                                });
                            });
                        column.Item().
                            Row(row =>
                            {
                                row.RelativeItem().AlignLeft().Column(column =>
                                {
                                    column.Item().Text("Date").FontSize(11);
                                });
                                row.RelativeItem().AlignRight().Column(column =>
                                {
                                    column.Item().AlignRight().Text(Model.IssueDate.ToString("yyyy/MM/dd")).FontSize(11);
                                });
                            });
                        column.Item().
                            Row(row =>
                            {
                                row.RelativeItem().AlignLeft().Column(column =>
                                {
                                    column.Item().Text("Due Date").FontSize(11);
                                });
                                row.RelativeItem().AlignRight().Column(column =>
                                {
                                    column.Item().Text(Model.DueDate.ToString("yyyy/MM/dd")).FontSize(11);
                                });
                            });

                    });
                });
                column.Item().Row(row =>
                {
                    row.RelativeItem().AlignLeft().Column(column =>
                    {
                        column.Item().Text("").FontSize(11);
                        column.Item().Text("Invoice to:").FontSize(11);
                        column.Item().Text(Model.AddressAsAString).FontSize(12).LineHeight(1f);

                    });
                });
            });
        }


        void ComposeTable(QuestPDF.Infrastructure.IContainer container)
        {
            container.Table(table =>
            {

                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });


                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#").FontSize(10);
                    header.Cell().Element(CellStyle).Text("Product").FontSize(10);
                    header.Cell().Element(CellStyle).Text("Description").FontSize(10);
                    header.Cell().Element(CellStyle).AlignCenter().Text("Qty").FontSize(10);
                    header.Cell().Element(CellStyle).AlignRight().Text("Unit price").FontSize(10);
                    header.Cell().Element(CellStyle).AlignRight().Text("Total").FontSize(10);
                    static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.Bold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                int i = 0;
                foreach (var item in Model.Items)
                {

                    table.Cell().Element(cell => CellStyle(cell, i, Model.Items.Count)).Text(Model.Items.IndexOf(item) + 1).FontSize(11);
                    table.Cell().Element(cell => CellStyle(cell, i, Model.Items.Count)).Text(item.Name).FontSize(9);
                    table.Cell().Element(cell => CellStyle(cell, i, Model.Items.Count)).Text(item.Description).FontSize(7);
                    table.Cell().Element(cell => CellStyle(cell, i, Model.Items.Count)).AlignRight().AlignCenter().Text(item.Quantity).FontSize(12);
                    table.Cell().Element(cell => CellStyle(cell, i, Model.Items.Count)).AlignRight().Text($"{Model.CurrencySymbol}{item.Price.ToString("0.00")}").FontSize(9);
                    table.Cell().Element(cell => CellStyle(cell, i, Model.Items.Count)).AlignRight().Text($"{Model.CurrencySymbol}{(item.Price * item.Quantity).ToString("0.00")}").FontSize(9);
                    i++;

                    static QuestPDF.Infrastructure.IContainer CellStyle(QuestPDF.Infrastructure.IContainer container, int rowIndex, int total)
                    {
                        string colour = rowIndex % 2 == 1 ? "#DDDDDD" : "#FFFFFF";
                        int borderbottom = rowIndex == total - 1 ? 0 : 1;
                        return container.Background(colour).BorderBottom(borderbottom).BorderColor(Colors.Grey.Lighten2).PaddingVertical(1);
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
                });
            });
        }

        void ComposeContent(QuestPDF.Infrastructure.IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);
                column.Item().Element(ComposeTable);

                column.Spacing(5);
                column.Item().AlignRight().Row(row =>
                {

                    row.RelativeItem().AlignLeft();

                    row.RelativeItem().AlignRight().Column(column =>
                    {
                        if (Model.IsVat)
                        {
                            column.Item().AlignLeft().Row(row =>
                            {
                                row.RelativeItem().Text("Includes VAT Total").FontSize(11).SemiBold();
                                row.RelativeItem().AlignRight().Text(Model.CurrencySymbol + (Model.VATpaid).ToString("0.00")).FontSize(12);
                            });
                        }
                        column.Item().AlignLeft().Row(row =>
                        {
                            row.RelativeItem().Text("Total").FontSize(11).SemiBold();
                            row.RelativeItem().AlignRight().Text(Model.CurrencySymbol + (Model.Total).ToString("0.00")).FontSize(12);
                        });
                        column.Item().AlignLeft().Row(row =>
                        {
                            row.RelativeItem().Text("Payment").FontSize(11).SemiBold();
                            row.RelativeItem().AlignRight().Text(Model.CurrencySymbol + (Model.Total).ToString("0.00")).FontSize(12);
                        });
                        column.Item().AlignLeft().Row(row =>
                        {
                            row.RelativeItem().Text("Balance due").FontSize(11).SemiBold();
                            row.RelativeItem().AlignRight().Text(Model.CurrencySymbol + "0").FontSize(12);
                        });
                    });

                });

                if (Model.IsVat)
                {

                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Text("VAT SUMMARY");
                    });
                    column.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Black);
                    column.Spacing(5);
                    column.Item().AlignRight().Row(row =>
                    {

                        row.RelativeItem().AlignLeft();

                        row.RelativeItem().AlignRight().Column(column =>
                        {
                            column.Item().AlignLeft().Row(row =>
                            {
                                row.RelativeItem().Text("VAT rate").FontSize(11);
                                row.RelativeItem().AlignRight().Text(Model.VatRate + "%").FontSize(9);
                            });

                            column.Item().AlignLeft().Row(row =>
                            {
                                row.RelativeItem().Text("VAT").FontSize(11);
                                row.RelativeItem().AlignRight().Text(Model.CurrencySymbol + (Model.VATpaid).ToString("0.00")).FontSize(9);
                            });
                            column.Item().AlignLeft().Row(row =>
                            {
                                row.RelativeItem().Text("NET").FontSize(11);
                                row.RelativeItem().AlignRight().Text(Model.CurrencySymbol + (Model.PriceNet).ToString("0.00")).FontSize(9);
                            });
                        });

                    });

                }

            });

        }
    }

    public async static Task<InvoiceModel> GetInvoiceDetails(Complete komplet, InvoicePrintoutDataPack invoicePrintoutDataPack)
    {
        var zwrotka = new InvoiceModel();
        zwrotka.InvoiceNumber = komplet.Order.orderID;
        zwrotka.IsVat = komplet.Order.VAT;
        zwrotka.IssueDate = komplet.Order.paidOn;
        zwrotka.DueDate = komplet.Order.paidOn;
        zwrotka.VatRate = invoicePrintoutDataPack.VatRates[komplet.Order.orderID];
        zwrotka.Total =komplet.Order.saletotal;
        var kantry = invoicePrintoutDataPack.kantry[komplet.BillAddr.CountryCode];
        StringBuilder name = new StringBuilder();
        if (komplet.Customer.Title != null) name.Append( komplet.Customer.Title + ' ');
        if (komplet.Customer.GivenName != null) name.Append(komplet.Customer.GivenName + ' ');
        if (komplet.Customer.MiddleName != null) name.Append(komplet.Customer.MiddleName + ' ');
        if (komplet.Customer.FamilyName != null) name.Append(komplet.Customer.FamilyName);
        StringBuilder addressasastring =new StringBuilder(name.ToString() + Environment.NewLine);
        if (string.IsNullOrEmpty(komplet.BillAddr.AddressAsAString))
        {
            if (!string.IsNullOrEmpty(komplet.Customer.CompanyName))
            {
                addressasastring.Append(komplet.Customer.CompanyName + Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(komplet.BillAddr.Line1))
            {
                addressasastring.Append(komplet.BillAddr.Line1 + Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(komplet.BillAddr.Line2))
            {
                addressasastring.Append(komplet.BillAddr.Line2 + Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(komplet.BillAddr.City))
            {
                addressasastring.Append(komplet.BillAddr.City + Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(komplet.BillAddr.PostalCode))
            {
                addressasastring.Append(komplet.BillAddr.PostalCode + Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(komplet.BillAddr.CountrySubDivisionCode))
            {
                addressasastring.Append(komplet.BillAddr.CountrySubDivisionCode + Environment.NewLine);
            }
            if (komplet.BillAddr.CountryCode != null)
            {
                addressasastring.Append(kantry);
            }
        }
        else
        {
            addressasastring.Clear();addressasastring.Append(komplet.BillAddr.AddressAsAString);
        }
        zwrotka.AddressAsAString =addressasastring.ToString(). Trim();
        zwrotka.CurrencySymbol = invoicePrintoutDataPack.currencies[komplet.Order.orderID].symbol;
        if (!zwrotka.IsVat)
        {
            zwrotka.VATpaid = 0;
            zwrotka.PriceNet = Math.Round(Convert.ToDecimal(komplet.Order.saletotal), 2);
        }
        else
        {
            zwrotka.PriceNet = Math.Round(komplet.Order.saletotal /(1+(invoicePrintoutDataPack.VatRates[komplet.Order.orderID]/100)) , 2);
            zwrotka.VATpaid = Math.Round(Convert.ToDecimal(komplet.Order.saletotal) - zwrotka.PriceNet, 2);
        }
        zwrotka.Items = new();
        List<itembody> bodies = new();
        Dictionary<int, string> typki = new();
        Dictionary<int, itmmarketassoc> itmMrkAssocs = new ();
        Dictionary<int, itembody> soldWithS = new();
        Dictionary<int, string> kolorki = new();
            
        foreach (var oi in komplet.OrderItems)
        {
            var body = invoicePrintoutDataPack.items[oi.itembodyID].itembody;
            var brand = invoicePrintoutDataPack.Brands[body.brandID];
            var itema = new QPDFOrderItems();

            bodies.Add(body);

            if (!body.mpn.Equals("Tool"))
            {
                var typek = invoicePrintoutDataPack.types[body.typeId];

                if (!typki.ContainsKey(body.itembodyID))
                {
                    typki.Add(body.itembodyID, typek);
                }

                if (oi.itmMarketAssID != null)
                {
                    var ima = invoicePrintoutDataPack.items[oi.itembodyID].ItmMarketAssocs.First(p => p.itmmarketassID == oi.itmMarketAssID);

                    if (!itmMrkAssocs.ContainsKey(body.itembodyID))
                    {
                        itmMrkAssocs.Add(body.itembodyID, ima);
                        if (ima.soldWith != null)
                        {
                            soldWithS.Add(body.itembodyID, invoicePrintoutDataPack.items[(int)ima.soldWith].itembody);
                        }
                    }
                }
                if (typek.Equals("Strap") || typek.Equals("Bezel"))
                {
                    var kolorBodyEntry = invoicePrintoutDataPack.items[oi.itembodyID].ItmCechies
                        .FirstOrDefault(p => p.itembodyID == body.itembodyID && p.parameterID == invoicePrintoutDataPack.kolorId);
                    int kolorBody = -1;
                    if (kolorBodyEntry != null)
                    {
                        kolorBody = kolorBodyEntry.parameterValueID;
                        // Proceed with kolorBody
                    }
                    else
                    {
                        // TODO: Handle the case when kolorBodyEntry is null
                        throw new Exception($"{body.myname} with mpn {body.mpn} is of a type {typek} and types Strap and Bezel must have associated property colour set");
                    }
                    var kolor = invoicePrintoutDataPack.cechyValues[invoicePrintoutDataPack.kolorId].First(p => p.parameterValueID == kolorBody).name;
                    if (!kolorki.ContainsKey(body.itembodyID))
                    {
                        kolorki.Add(body.itembodyID, kolor);
                    }
                }
                if (typek.Equals("Screw"))
                {
                    itmmarketassoc asso;
                    if (oi.itmMarketAssID != null)
                    {
                        asso = itmMrkAssocs[body.itembodyID];
                    }
                    else
                    {
                        asso = new itmmarketassoc
                        {
                            quantitySold = 1
                        };
                    }
                    int ilo = asso.quantitySold;
                    int sowi = -1;
                    itembody soldW = new ();
                    if (asso.soldWith != null)
                    {
                        soldW = soldWithS[body.itembodyID];
                        sowi = soldW.itembodyID;
                    }
                    string desc = ilo + "x" + body.mpn;
                    if (sowi != -1)
                    {
                        desc += ", " + ilo + "x" + soldW.mpn;
                    }
                    itema.Description = desc;
                }
                else if (typek.Equals("Strap"))
                {
                    itema.Description = brand + ", " + "MPN:" + body.mpn + ", Colour:" + kolorki[body.itembodyID];
                }
                else
                {
                    itema.Description = brand + ", " + "MPN:" + body.mpn + ", " + typek;
                }
            }
            itema.Price = Math.Round(Convert.ToDecimal(oi.price), 2);
            itema.Name = oi.itemName;
            itema.Quantity = oi.quantity;
            zwrotka.Items.Add(itema);
        }
        if (komplet.Order.postagePrice > 0)
        {
            var itema = new QPDFOrderItems();
            itema.Name = invoicePrintoutDataPack.postageTypes[komplet.Order.postageType];
            itema.Quantity = 1;
            itema.Description = "";
            itema.Price = Math.Round(Convert.ToDecimal(komplet.Order.postagePrice), 2);
            zwrotka.Items.Add(itema);
        }
        zwrotka.MarketName = invoicePrintoutDataPack.markety[komplet.Order.market];
        return zwrotka;
    }

    public async Task<MemoryStream> GenerateStream(List<Complete> Komplety, InvoicePrintoutDataPack invoicePrintoutDataPack)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        int i = 0;
        var dokumenty = new List<InvoiceDocument>();
        foreach (var unused in Komplety)
        {
            var model = await GetInvoiceDetails(Komplety[i], invoicePrintoutDataPack);
            dokumenty.Add(new InvoiceDocument(model));
            i++;
        }
        MemoryStream myStream = new MemoryStream();
        QuestPDF.Fluent.Document.Merge(dokumenty).GenerateXps(myStream);
        return myStream;
    }
}