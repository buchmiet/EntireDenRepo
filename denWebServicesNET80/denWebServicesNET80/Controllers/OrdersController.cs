using AmazonSPAPIClient;
using DataServicesNET80;
using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Extensions;
using DataServicesNET80.Models;
using denEbayNET80;
using denQuickbooksNET80;
using denSharedLibrary;
using denWebservicesNET80;
using Microsoft.AspNetCore.Mvc;

namespace denWebServicesNET80.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrdersController(IDatabaseAccessLayer databaseAccessLayer, IMarketActions marketActions,
        IQuickBooksService quickBooksService, IAmazonSpApi amazonSpApi, ILogger<OrdersController> logger)
    : ControllerBase
{

    

    // Trasy (Routes) i nazwy akcji
    private const string RenameAction = "Rename";
    private const string LaunchRoute = "Launch/{id}";
    private const string LaunchAction = "Launch";

    // Ścieżki do plików i katalogów
    private const string WatchesRenameFilePath = @"C:\Inetpub\vhosts\time4parts.co.uk\watches\staretonowe.txt";
    private const string WatchesBaseDirectory = @"C:\Inetpub\vhosts\time4parts.co.uk\watches\";
    private const string WatchesHalfDirectory = @"C:\Inetpub\vhosts\time4parts.co.uk\watches\half\";

    // Statusy, stany i komunikaty
    private const string StatusOK = "OK";
    private const string StatusInProgress = "dzialamy";
    private const string StatusLaunched = "odpalilem";
    private const string OrderStatusNew = "_NEW";
    private const string OrderStatusError = "_ERR";
    private const string EbayFulfillmentStatusNotStarted = "NOT_STARTED";
    private const string EbayPaymentStatusPaid = "PAID";
    private const string EbayPaymentStatusPartiallyRefunded = "PARTIALLY_REFUNDED";

    // Identyfikatory, nazwy i wartości w logice
    private const string ItemTypeName = "Item";
    private const string ToolItemName = "Tool";
    private const string WithToolSku = "withTool";
    private const string EbayUkMarketName = "ebayuk";
    private const string EbayEsMarketName = "ebayES";
    private const string GbpCurrencyCode = "GBP";
    private const string VridOrderNote = "vrid";
    private const string SpainCountryCode = "ES";
    private const string CanaryIslandsPostalPrefix1 = "35";
    private const string CanaryIslandsPostalPrefix2 = "38";

    // Identyfikatory platform i rynków
    private const string EbayPlatformString = "ebay";
    private const string EbayPlatformStringWithSpace = " ebay"; // Używane do czyszczenia adresów

    // Twardo zakodowane identyfikatory zamówień do wykluczenia
    private const string ExcludedEbayOrderId1 = "07-04212-69411";
    private const string ExcludedEbayOrderId2 = "04-04990-07685";

    // Typy przesyłek
    private const string PostageTypeRoyalMailFirstClass = "RM1ST";
    private const string PostageTypeRoyalMailInternational = "RMINT";


    public static int LocationId = 1;
    private static readonly SemaphoreSlim SemaphoreSlim = new(1, 1);
    private static Task _runningTask = Task.CompletedTask;


    [HttpGet(RenameAction, Name = RenameAction)]
    public IActionResult Rename()
    {
        var linijki = System.IO.File.ReadAllLines(WatchesRenameFilePath);
        var dicto = new Dictionary<string, string>();
        foreach (var line in linijki)
        {
            var tsa = line.Split(',');
            dicto.Add(tsa[0], tsa[1]);
        }

        foreach (var zeku in dicto)
        {
            if (System.IO.File.Exists(WatchesBaseDirectory + zeku.Key) &&
                !System.IO.File.Exists(WatchesBaseDirectory + zeku.Value))
                System.IO.File.Move(WatchesBaseDirectory + zeku.Key,
                    WatchesBaseDirectory + zeku.Value);
            if (System.IO.File.Exists(WatchesHalfDirectory + zeku.Key) &&
                !System.IO.File.Exists(WatchesHalfDirectory + zeku.Value))
                System.IO.File.Move(WatchesHalfDirectory + zeku.Key,
                    WatchesHalfDirectory + zeku.Value);
        }

        return Ok(StatusOK);
    }

    [HttpGet(LaunchRoute, Name = LaunchAction)]
    public IActionResult Launch(int id)
    {
        // Sprawdzenie, czy inna operacja już trwa.
        if (!_runningTask.IsCompleted)
            // Jeśli operacja już trwa, zwróć "dzialamy".
            return Ok(StatusInProgress);

        // Zajęcie semafora.
        SemaphoreSlim.Wait();

        // Rozpoczęcie nowego zadania.
        _runningTask = Task.Run(async () =>
        {
            try
            {
                LocationId = id;

                await GetEbayOrders2(LocationId);

                if (LocationId == 1) await doAmazonOrders(LocationId);
            }
            catch (Exception ex)
            {
                // Logika obsługi wyjątków.
            }
            finally
            {
                // Zwolnienie semafora po zakończeniu zadania.
                SemaphoreSlim.Release();
            }
        });

        // Zwrócenie odpowiedzi, że proces został uruchomiony.
        return Ok(StatusLaunched);
    }

    public KeyValuePair<customer, billaddr> EBay2Custotomer2MyCustomer(ebayOrders2.Order o)
    {
        var kli = o.fulfillmentStartInstructions.First().shippingStep.shipTo;
        var billAddr = new billaddr
        {
            Line1 = kli.contactAddress.addressLine1,
            Line2 = kli.contactAddress.addressLine2,
            City = kli.contactAddress.city,
            PostalCode = kli.contactAddress.postalCode,
            CountrySubDivisionCode = kli.contactAddress.stateOrProvince,
            CountryCode = kli.contactAddress.countryCode
        };
        if (!string.IsNullOrEmpty(billAddr.Line1))
        {
            if (billAddr.Line1.StartsWith(EbayPlatformString)) billAddr.Line1 = "";
            if (billAddr.Line1.Contains(EbayPlatformStringWithSpace))
                billAddr.Line1 = billAddr.Line2[..billAddr.Line1.IndexOf(EbayPlatformStringWithSpace)];
        }

        if (!string.IsNullOrEmpty(billAddr.Line2))
        {
            if (billAddr.Line2.StartsWith(EbayPlatformString)) billAddr.Line2 = "";
            if (billAddr.Line2.Contains(EbayPlatformStringWithSpace))
                billAddr.Line2 = billAddr.Line2[..billAddr.Line2.IndexOf(EbayPlatformStringWithSpace)];
        }

        if (billAddr.City == null) billAddr.City = "";
        if (billAddr.CountrySubDivisionCode == null) billAddr.CountrySubDivisionCode = "";
        if (billAddr.PostalCode == null) billAddr.PostalCode = "";
        char[] charsy = { ' ' };
        var names = new string[2];
        string[] nazwy;

        if (string.IsNullOrEmpty(kli.fullName))
        {
            nazwy = new string[2];
            nazwy[0] = "";
            nazwy[1] = "";
        }
        else
        {
            nazwy = kli.fullName.Split(charsy, 2);
        }

        names[0] = nazwy[0];
        if (nazwy.Length == 2)
            names[1] = nazwy[1];
        else names[1] = "";

        var cust = new customer
        {
            CompanyName = "",
            Title = "",
            GivenName = names[0],
            MiddleName = "",
            FamilyName = names[1],
            DisplayName = names[0] + ' ' + names[1] + ' ' + o.buyer.username,
            Email = kli.email,
            Phone = kli.primaryPhone.phoneNumber,
            currency = o.paymentSummary.payments[0].amount.currency,
            billaddrID = billAddr.billaddrID
        };
        return new KeyValuePair<customer, billaddr>(cust, billAddr);
    }

    public void Emptyshot(string s)
    {
    }

    public async Task GoodQuantitiesChanger(Dictionary<int, int> q2u, int locationID)
    {
        foreach (var wartosc in q2u)
            await marketActions.AssignQuantities2Markets(wartosc.Key, locationID, wartosc.Value, Emptyshot);
    }

    private async Task GetEbayOrders2(int locationID)
    {
        var tx = await marketActions.FetchEbayOrders2(Emptyshot, locationID);
        var s = tx.orders;
        var orderyWsie = s.Where(p =>
            p.orderFulfillmentStatus.Equals(EbayFulfillmentStatusNotStarted) && (p.orderPaymentStatus.Equals(EbayPaymentStatusPaid)|| p.orderPaymentStatus.Equals(EbayPaymentStatusPartiallyRefunded)) &&
            !p.orderId.Equals(ExcludedEbayOrderId1) && !p.orderId.Equals(ExcludedEbayOrderId2)).ToList();
        List<ebayOrders2.Order> gogo = [];
        orderitemtype itemType = new();
        var markety = (await databaseAccessLayer.markety()).Select(p => p.Value).ToList();
        itembody ToolBody = new();
        using (var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext()))
        {
            var itemTypeService = new EntityService<orderitemtype>(unitOfWork);
            var invoiceTXNService = new EntityService<invoicetxn>(unitOfWork);
            var ItemBodyService = new EntityService<itembody>(unitOfWork);
            itemType = await itemTypeService.GetOneAsync(p => p.name.Equals(ItemTypeName));
            ToolBody = await ItemBodyService.GetOneAsync(p => p.mpn.Equals(ToolItemName));
            foreach (var ord in orderyWsie)
            {
                var existingOrd = await invoiceTXNService.GetOneAsync(p => p.platformTXN.Equals(ord.orderId));
                if (existingOrd == null) gogo.Add(ord);
            }
        }

        orderyWsie = gogo;
        var DownloadedOrders = new List<Complete>();
        var quantities2Update = new Dictionary<int, int>();
        var quantitiesSold = new List<intintdouble>();
        var noweordy = 0;
        foreach (var o in orderyWsie)
            if (Convert.ToDouble(o.paymentSummary.totalDueSeller.value) > 0)
            {
                var dziad = EBay2Custotomer2MyCustomer(o);
                int thismarket;
                if (o.pricingSummary.total.convertedFromCurrency == null)
                    thismarket = markety.First(p => p.name.ToLower().Equals(EbayUkMarketName)).marketID;
                else
                    thismarket = markety.First(p => p.name.Equals(EbayEsMarketName)).marketID;

                var zamo = new order
                {
                    locationID = locationID, 
                    salecurrency = o.pricingSummary.total.currency,
                    acquiredcurrency = GbpCurrencyCode,
                    paidOn = Convert.ToDateTime(o.creationDate),
                    dispatchedOn = Convert.ToDateTime(o.creationDate),
                    tracking = "",
                    market = thismarket,
                    status = OrderStatusNew,
                    saletotal = Convert.ToDecimal(o.pricingSummary.total.value),
                    VAT = true,
                    order_notes = VridOrderNote,
                    VATRateID = 1,
                    xchgrate = 1
                };
                if (o.pricingSummary.priceSubtotal.value != o.pricingSummary.total.value)
                    if (!dziad.Value.PostalCode.StartsWith(CanaryIslandsPostalPrefix1) && !dziad.Value.PostalCode.StartsWith(CanaryIslandsPostalPrefix2))
                    {
                        zamo.VAT = false;
                        zamo.VATRateID = 3;
                    }

                if (o.pricingSummary.total.convertedFromCurrency != null)
                    zamo.salecurrency = o.pricingSummary.total.convertedFromCurrency;

                decimal PostagePrice = 0;
                var ReadyComplete = new Complete();
                var orderItems = new List<orderitem>();
                foreach (var przedm in o.lineItems)
                {
                    itmmarketassoc ProductSoldOnMarket = new();
                    itembody bod = new();

                    using (var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext()))
                    {
                        var itmMarketAssocService = new EntityService<itmmarketassoc>(unitOfWork);
                        var logentryService = new EntityService<logentry>(unitOfWork);
                        var ItemBodyService = new EntityService<itembody>(unitOfWork);
                        ProductSoldOnMarket =
                            await itmMarketAssocService.GetOneAsync(p => p.itemNumber.Equals(przedm.legacyItemId));
                        if (ProductSoldOnMarket == null)
                        {
                            var logev2 = new logentry
                            {
                                _event = przedm.legacyItemId + " not associated with any product in the DB",
                                eventdate = DateTime.Now
                            };
                            await logentryService.AddAsync(logev2);
                        }
                        else
                        {
                            bod = await ItemBodyService.GetOneAsync(p =>
                                p.itembodyID == ProductSoldOnMarket.itembodyID);
                        }
                    }

                    if (ProductSoldOnMarket != null)
                    {
                        if (przedm.sku == null)
                        {
                            var linijka = new orderitem
                            {
                                itembodyID = bod.itembodyID,
                                itemName = bod.name,
                                quantity = Convert.ToInt32(przedm.quantity),
                                price = Convert.ToDecimal(przedm.lineItemCost.value) / Convert.ToInt32(przedm.quantity),
                                OrderItemTypeId = itemType.OrderItemTypeId,
                                itmMarketAssID = ProductSoldOnMarket.itmmarketassID,
                                ItemWeight = bod.weight
                            };
                            orderItems.Add(linijka);
                            if (bod.readyTotrack)
                            {
                                var multip = ProductSoldOnMarket.quantitySold;
                                noweordy++;
                                quantitiesSold.Add(new intintdouble
                                {
                                    itembodyid = bod.itembodyID,
                                    quantityprice = new intdouble
                                    {
                                        quantity = linijka.quantity * multip,
                                        price = (double)linijka.price
                                    }
                                });
                                if (!quantities2Update.ContainsKey(bod.itembodyID))
                                    quantities2Update.Add(bod.itembodyID, linijka.quantity * multip);
                                else
                                    quantities2Update[bod.itembodyID] += linijka.quantity * multip;
                            }
                        }
                        else
                        {
                            if (przedm.sku == WithToolSku)
                            {
                                var linijka = new orderitem
                                {
                                    itembodyID = bod.itembodyID,
                                    itemName = bod.name,
                                    quantity = Convert.ToInt32(przedm.quantity),
                                    price = (Convert.ToDecimal(przedm.lineItemCost.value) -
                                             Convert.ToInt32(przedm.quantity)) / Convert.ToInt32(przedm.quantity),
                                    OrderItemTypeId = itemType.OrderItemTypeId,
                                    itmMarketAssID = ProductSoldOnMarket.itmmarketassID,
                                    ItemWeight = bod.weight
                                };
                                orderItems.Add(linijka);

                                if (bod.readyTotrack && zamo.locationID == 1)
                                {
                                    var multip = ProductSoldOnMarket.quantitySold;
                                    quantitiesSold.Add(new intintdouble
                                    {
                                        itembodyid = bod.itembodyID,
                                        quantityprice = new intdouble
                                        {
                                            quantity = linijka.quantity * multip,
                                            price = (double)linijka.price
                                        }
                                    });
                                    if (!quantities2Update.ContainsKey(bod.itembodyID))
                                        quantities2Update.Add(bod.itembodyID, linijka.quantity * multip);
                                    else
                                        quantities2Update[bod.itembodyID] += linijka.quantity * multip;
                                }

                                var toolLinijka = new orderitem
                                {
                                    itembodyID = ToolBody.itembodyID,
                                    itemName = ToolItemName,
                                    quantity = Convert.ToInt32(przedm.quantity),
                                    price = 1,
                                    OrderItemTypeId = itemType.OrderItemTypeId,
                                    orderID = zamo.orderID,
                                    ItemWeight = 5
                                };
                                orderItems.Add(toolLinijka);
                            }
                            else
                            {
                                var linijka = new orderitem
                                {
                                    itembodyID = bod.itembodyID,
                                    itemName = bod.name,
                                    quantity = Convert.ToInt32(przedm.quantity),
                                    price = Convert.ToDecimal(przedm.lineItemCost.value) /
                                            Convert.ToInt32(przedm.quantity),
                                    OrderItemTypeId = itemType.OrderItemTypeId,

                                    itmMarketAssID = ProductSoldOnMarket.itmmarketassID,
                                    ItemWeight = bod.weight
                                };

                                orderItems.Add(linijka);
                                if (bod.readyTotrack && zamo.locationID == locationID)
                                {
                                    var multip = ProductSoldOnMarket.quantitySold;

                                    quantitiesSold.Add(new intintdouble
                                    {
                                        itembodyid = bod.itembodyID,
                                        quantityprice = new intdouble
                                        {
                                            quantity = linijka.quantity * multip,
                                            price = (double)linijka.price
                                        }
                                    });

                                    if (!quantities2Update.ContainsKey(bod.itembodyID))
                                        quantities2Update.Add(bod.itembodyID, linijka.quantity * multip);
                                    else
                                        quantities2Update[bod.itembodyID] += linijka.quantity * multip;
                                }
                            }
                        }
                    }
                    else
                    {
                        zamo.status = OrderStatusError;
                    }

                    PostagePrice = Convert.ToDecimal(o.pricingSummary.deliveryCost.value);
                    if (o.pricingSummary.deliveryDiscount != null)
                        PostagePrice += Convert.ToDecimal(o.pricingSummary.deliveryDiscount.value);
                }

                ReadyComplete = await databaseAccessLayer.AddComplete(zamo, dziad.Key, dziad.Value, orderItems);
                using (var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext()))
                {
                    var orderService = new orderService(unitOfWork);
                    var invoiceTXNService = new EntityService<invoicetxn>(unitOfWork);
                    zamo = await orderService.GetOneAsync(p => p.orderID == ReadyComplete.Order.orderID);
                    if (PostagePrice == 0)
                        zamo.postageType = PostageTypeRoyalMailFirstClass;
                    else
                        zamo.postageType = PostageTypeRoyalMailInternational;
                    zamo.postagePrice = PostagePrice;
                    await orderService.UpdateAsync(zamo);
                    if (zamo.status != OrderStatusError)
                    {
                        var invtxn = new invoicetxn
                        {
                            platformID = 1,
                            platformTXN = o.orderId,
                            marketID = thismarket,
                            orderID = zamo.orderID
                        };
                        await invoiceTXNService.AddAsync(invtxn);
                    }
                    ReadyComplete.Order = zamo;
                    DownloadedOrders.Add(ReadyComplete);
                }
            }

        await GoodQuantitiesChanger(quantities2Update, locationID);
        await AddOrder2QuickBooks(DownloadedOrders, itemType);
    }

    public async Task doAmazonOrders(int locationID)
    {
        Dictionary<string, decimal> vaty = new();

        orderitemtype itemType = new();
        GetOrdersResponse AmazonOrders = new();
        Dictionary<string, int> marketplaces = null;
        var zamowionka = new List<GetOrdersResponse.Order>();
        using (var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext()))
        {
            var itemTypeService = new EntityService<orderitemtype>(unitOfWork);
            var AmazonMarketplaceService = new EntityService<amazonmarketplace>(unitOfWork);
            var invoiceTXNService = new EntityService<invoicetxn>(unitOfWork);
            var vatService = new EntityService<countryvatrrate>(unitOfWork);
            marketplaces = (await AmazonMarketplaceService.GetAllAsync()).ToDictionary(p => p.code, q => q.marketID);
            vaty = (await vatService.GetAllAsync()).ToDictionary(p => p.code, q => q.rate);
            var xxxx = await amazonSpApi.GetParticularOrders(marketplaces.Keys.ToList(), ["403-2592097-7793168"], Emptyshot, 1);
            AmazonOrders = await amazonSpApi.   GetNewOrders(marketplaces.Keys.ToList(),
                Emptyshot, locationID);
            itemType = await itemTypeService.GetOneAsync(p => p.name.Equals(ItemTypeName));
            foreach (var order in AmazonOrders.Payload.Orders)
            {
                var existingOrd = await invoiceTXNService.GetOneAsync(p => p.platformTXN.Equals(order.AmazonOrderId));
                if (existingOrd == null) zamowionka.Add(order);
            }
        }

        if (zamowionka.Count == 0) return;

        var quantities2Update = new Dictionary<int, int>();
        var DownloadedOrders = new List<Complete>();

        foreach (var order in zamowionka)
        {
            var gx = order.MarketplaceId;
            var thisMarket = marketplaces[gx];
            var klient = amazonSpApi.AmazonCustomer2MyCustomer(order);
            var billaddr = klient.Value;
            var customer = klient.Key;

            var przedmy = await amazonSpApi.GetOrderItems(order.AmazonOrderId, Emptyshot, locationID);
            decimal vatdivider = 1;

            var zamo = new order
            {
                locationID = locationID,
                salecurrency = customer.currency,
                acquiredcurrency = GbpCurrencyCode,
                paidOn = order.PurchaseDate,
                dispatchedOn = order.PurchaseDate,
                tracking = "",
                market = thisMarket,
                status = OrderStatusNew,
                saletotal = Convert.ToDecimal(order.OrderTotal.Amount),
                VAT = true,
                order_notes = "",
                VATRateID = 1,
                xchgrate = await Xrates.getXrate(order.PurchaseDate, customer.currency)
            };

            if (vaty.TryGetValue(order.ShippingAddress.CountryCode, out var value))
                if (!
                    (klient.Value.CountryCode.ToUpper().Equals(SpainCountryCode) &&
                     klient.Value.PostalCode.StartsWith(CanaryIslandsPostalPrefix1) && !klient.Value.PostalCode.StartsWith(CanaryIslandsPostalPrefix2)
                    )
                   )

                {
                    zamo.VAT = false;
                    vatdivider = 1 + value / 100;
                    zamo.saletotal /= vatdivider;
                    zamo.VATRateID = 3;
                }
            //canary islands check end

            decimal PostagePrice = 0;
            var ReadyComplete = new Complete();
            var orderItems = new List<orderitem>();
            foreach (var przedm in przedmy.OrderItems)
            {
                itmmarketassoc? productSoldOnMarket;
                itembody bod = new();

                using (var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext()))
                {
                    var itmMarketAssocService = new EntityService<itmmarketassoc>(unitOfWork);
                    var logentryService = new EntityService<logentry>(unitOfWork);
                    var ItemBodyService = new EntityService<itembody>(unitOfWork);
                    productSoldOnMarket =
                        await itmMarketAssocService.GetOneAsync(p => p.itemNumber.Equals(przedm.ASIN));
                    if (productSoldOnMarket == null)
                    {
                        var logev2 = new logentry
                        {
                            _event = przedm.ASIN + " not associated with any product in the DB",
                            eventdate = DateTime.Now
                        };
                        await logentryService.AddAsync(logev2);
                    }
                    else
                    {
                        bod = await ItemBodyService.GetOneAsync(p => p.itembodyID == productSoldOnMarket.itembodyID);
                    }
                }

                if (productSoldOnMarket != null)
                {
                    var kuk1 = Convert.ToDecimal(przedm.ItemPrice.Amount) / przedm.QuantityOrdered;
                    var kuk2 = kuk1 / vatdivider;

                    var linijka = new orderitem
                    {
                        itembodyID = bod.itembodyID,
                        itemName = bod.name,
                        quantity = przedm.QuantityOrdered,
                        price = kuk2,
                        OrderItemTypeId = itemType.OrderItemTypeId,
                        itmMarketAssID = productSoldOnMarket.itmmarketassID,
                        ItemWeight = bod.weight
                    };
                    orderItems.Add(linijka);

                    if (bod.readyTotrack)
                    {
                        var multip = productSoldOnMarket.quantitySold;
                        if (!quantities2Update.ContainsKey(bod.itembodyID))
                            quantities2Update.Add(bod.itembodyID, linijka.quantity * multip);
                        else
                            quantities2Update[bod.itembodyID] += linijka.quantity * multip;
                    }

                    decimal pro = 0;
                    if (przedm.ShippingPrice != null) pro = Convert.ToDecimal(przedm.ShippingPrice.Amount) / vatdivider;
                    PostagePrice += pro;
                }
                else
                {
                    zamo.status = OrderStatusError;
                }
            }

            ReadyComplete = await databaseAccessLayer.AddComplete(zamo, customer, billaddr, orderItems);
            using (var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext()))
            {
                var orderService = new orderService(unitOfWork);
                var invoiceTXNService = new EntityService<invoicetxn>(unitOfWork);
                zamo = await orderService.GetOneAsync(p => p.orderID == ReadyComplete.Order.orderID);
                if (PostagePrice == 0)
                    zamo.postageType = PostageTypeRoyalMailFirstClass;
                else
                    zamo.postageType = PostageTypeRoyalMailInternational;
                zamo.postagePrice = PostagePrice;
                await orderService.UpdateAsync(zamo);
                var invtxn = new invoicetxn
                {
                    platformID = 2,
                    platformTXN = order.AmazonOrderId,
                    marketID = thisMarket,
                    orderID = zamo.orderID
                };
                await invoiceTXNService.AddAsync(invtxn);
                ReadyComplete.Order = zamo;
                DownloadedOrders.Add(ReadyComplete);
            }
        }

        await GoodQuantitiesChanger(quantities2Update, locationID).ConfigureAwait(false);
        await AddOrder2QuickBooks(DownloadedOrders, itemType);
    }

    private async Task AddOrder2QuickBooks(List<Complete> DownloadedOrders, orderitemtype itemType)
    {
        
        foreach (var order in DownloadedOrders)
            if (!order.Order.status.Equals(OrderStatusError))
            {
                using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
                var orderService = new orderService(unitOfWork);
                var invoiceTXNService = new EntityService<invoicetxn>(unitOfWork);
                var itx = await invoiceTXNService.GetOneAsync(p => p.orderID == order.Order.orderID);
                if (LocationId != 1) continue;
                
                    var koko = await quickBooksService.ConvertMyInvoiceToQuickbooksInvoice(order, itemType, Emptyshot);
    
                    var invoiceResponse = await quickBooksService.AddInvoice(koko, Emptyshot, order.Order.xchgrate);
                    if (invoiceResponse is null)
                    {
                        logger.LogError("returnResponse.Error(invoiceResponse).GetMessages()");
                        return;
                    }



                    itx.qbInvoiceId = invoiceResponse.Id;
                    var ordo = await orderService.GetOneAsync(p => p.orderID == order.Order.orderID);
                    ordo.quickbooked = true;
                    await orderService.UpdateAsync(ordo);
                
            }
    }
}