using CommunityToolkit.Mvvm.Input;
using DataServicesNET80;
using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Extensions;
using DataServicesNET80.Models;

namespace denSharedLibrary;

public class CompletesActions
{
    public Dictionary<int, Complete> AllOrders = new();
    public Dictionary<int, invoicetxn> invtxns;

    public delegate void DataChangedEventHandler(List<int> changedData);

    public event DataChangedEventHandler CompletesChanged;

    private IDatabaseAccessLayer _databaseAccessLayer;
    private ICurrentKomplety _currentKomplety;

    public AsyncRelayCommand<CompletesChangedEventArgs> CompeletesChangedCommand { get; set; }

    public async Task CompletesChangedExecute(CompletesChangedEventArgs e)
    {
        var newords = await _databaseAccessLayer.GetKomplety(e.ChangedCompletes);
        foreach (var cd in newords)
        {
            if (!AllOrders.TryAdd(cd.Order.orderID, cd))
            {
                AllOrders[cd.Order.orderID] = cd;
            }
        }
        CompletesChanged?.Invoke(e.ChangedCompletes);
    }

    private void CompletesChangedHandler(object sender, CompletesChangedEventArgs e)
    {
        // Wywołaj asynchroniczną komendę
        CompeletesChangedCommand.ExecuteAsync(e);
    }

    public async Task AddInvTXN(int orderid)
    {
        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var invoicetxnService = new EntityService<invoicetxn>(unitOfWork);

        var newi = await invoicetxnService.GetOneAsync(p => p.orderID == orderid);
        invtxns.Add(orderid, newi);
    }

    public CompletesActions(IDatabaseAccessLayer databaseAccessLayer, ICurrentKomplety currentKomplety)
    {
        _databaseAccessLayer = databaseAccessLayer;
        _currentKomplety = currentKomplety;
        CompeletesChangedCommand = new AsyncRelayCommand<CompletesChangedEventArgs>(CompletesChangedExecute);
        _currentKomplety.CompletesChanged += CompletesChangedHandler;
    }

    public void HandleNewIncomingOrders(List<Complete> newOrders)
    {
        // Obsługuje nowe zamówienia
    }

    public async Task RefreshOrderInComplete(Complete komplet)
    {
        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var OrdersService = new orderService(unitOfWork);
        var ord = await OrdersService.GetOneAsync(p => p.orderID == komplet.Order.orderID);
        if (ord != null)
        {
            ord.paidOn = komplet.Order.paidOn;
            ord.dispatchedOn = komplet.Order.dispatchedOn;
            ord.status = komplet.Order.status;
            ord.salecurrency = komplet.Order.salecurrency;
            ord.acquiredcurrency = komplet.Order.acquiredcurrency;
            ord.tracking = komplet.Order.tracking;
            ord.postagePrice = komplet.Order.postagePrice;
            ord.saletotal = komplet.Order.saletotal;
            await OrdersService.UpdateAsync(ord);
            komplet.Order = ord;
        }

        await _currentKomplety.OrdersNeedRefreshing(new List<Complete> { komplet });
        AllOrders[komplet.Order.orderID] = komplet;
    }

    public async Task RefreshCustomerInComplete(Complete custTitleItem)
    {
        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var CustomerService = new EntityService<customer>(unitOfWork);
        var cust = await CustomerService.GetOneAsync(p => p.customerID == custTitleItem.Customer.customerID);
        if (cust != null)
        {
            cust.Title = custTitleItem.Customer.Title;
            cust.GivenName = custTitleItem.Customer.GivenName;
            cust.MiddleName = custTitleItem.Customer.MiddleName;
            cust.FamilyName = custTitleItem.Customer.FamilyName;
            cust.Email = custTitleItem.Customer.Email;
            cust.Phone = custTitleItem.Customer.Phone;
            await CustomerService.UpdateAsync(cust);
            custTitleItem.Customer = cust;
        }

        await _currentKomplety.OrdersNeedRefreshing(new List<Complete> { custTitleItem });
        AllOrders[custTitleItem.Order.orderID] = custTitleItem;
    }

    public async Task RefreshBillAddrInComplete(Complete komplecik)
    {
        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var BillAddrService = new EntityService<billaddr>(unitOfWork);
        var billAddr = await BillAddrService.GetOneAsync(p => p.billaddrID == komplecik.BillAddr.billaddrID);
        if (billAddr != null)
        {
            billAddr.Line1 = komplecik.BillAddr.Line1;
            billAddr.Line2 = komplecik.BillAddr.Line2;
            billAddr.City = komplecik.BillAddr.City;
            billAddr.PostalCode = komplecik.BillAddr.PostalCode;
            billAddr.CountrySubDivisionCode = komplecik.BillAddr.CountrySubDivisionCode;
            await BillAddrService.UpdateAsync(billAddr);
            komplecik.BillAddr = billAddr;
        }

        await _currentKomplety.OrdersNeedRefreshing(new List<Complete> { komplecik });
        AllOrders[komplecik.Order.orderID] = komplecik;
    }

    public async Task RefreshAmazonCustomerDetailsInComplete(Complete komplecik)
    {
        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var BillAddrService = new EntityService<billaddr>(unitOfWork);
        var CustomerService = new EntityService<customer>(unitOfWork);
        var cust = await CustomerService.GetOneAsync(p => p.customerID == komplecik.Customer.customerID);
        var billAddr = await BillAddrService.GetOneAsync(p => p.billaddrID == komplecik.BillAddr.billaddrID);
        cust.Email = komplecik.Customer.Email;
        billAddr.AddressAsAString = komplecik.BillAddr.AddressAsAString;
        await BillAddrService.UpdateAsync(billAddr);
        await CustomerService.UpdateAsync(cust);
        await _currentKomplety.OrdersNeedRefreshing(new List<Complete> { komplecik });
        AllOrders[komplecik.Order.orderID] = komplecik;
    }

    public async Task<List<int>> GetAllInvoicesAndOrderIds(int montsBack)
    {
        var dzien = DateTime.Now.AddMonths(-montsBack);
        invtxns = new Dictionary<int, invoicetxn>();

        using var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext());
        var ordersService = new orderService(unitOfWork);
        var invoiceTXNService = new EntityService<invoicetxn>(unitOfWork);
        var wszystkieordery = (await ordersService.GetAllSelectedColumnsAsync(p => p.paidOn >= dzien, p => p.orderID)).ToList();
        invtxns = (await invoiceTXNService.GetAllAsync(p => wszystkieordery.Contains(p.orderID))).ToDictionary(p => p.orderID, p => p);
        return wszystkieordery;
    }
}