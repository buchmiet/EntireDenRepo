using DataServicesNET80.DatabaseAccessLayer;
using DataServicesNET80.Models;

namespace denSharedLibrary;

public class CurrentKomplety(IDatabaseAccessLayer databaseAccessLayer) : ICurrentKomplety
{
    private Complete[] Komplety;
    public Dictionary<int, Complete> CompletesDict { get; private set; }


      

    public event EventHandler<CompletesChangedEventArgs> CompletesChanged;

    protected virtual void OnCompletesChanged(List<int> changedCompletes)
    {
        if (changedCompletes.Count > 0)
        {
            CompletesChanged?.Invoke(this, new CompletesChangedEventArgs {ChangedCompletes = changedCompletes});
        }
    }

    int _locationId;

    public Dictionary<int, int> retCusts { get; set; }

    public void SetLocation(int locationId)
    {
        _locationId = locationId;
    }
      

    public Complete[] GetKomplety()
    {
        if (Komplety == null)
        {
            return Array.Empty<Complete>();
        }
        return Komplety;
    }

    List<string> OrderStatuses = new() { "_NEW", "PROC" };

    public async Task<int> GetNumberOfOrders2Download()
    {
        return await databaseAccessLayer.GetNumberOfordersWithCondition(o=> OrderStatuses.Contains(o.status) && o.locationID == _locationId);
    }

    public async Task FetchKomplety(Dictionary<int, string> stareOrdy)
    {
        List<itembody> bodki = null;


        Komplety = (await databaseAccessLayer.GetKompletyWithGivenStatusesAndLocation(OrderStatuses, _locationId).ConfigureAwait(false)).ToArray();

        var itemy2 = Komplety.Select(p => p.OrderItems);

        List<orderitem> itemy3 = new();
        foreach (var itm in itemy2)
        {
            itemy3.AddRange(itm);
        }
        bodki = new List<itembody>();
        foreach (var itm in itemy3)
        {
            bodki.Add(databaseAccessLayer.items[itm.itembodyID].itembody);
        }
          
        retCusts = await databaseAccessLayer.CountordersForCustomers(Komplety.Select(k => k.Customer.customerID).ToList()).ConfigureAwait(false);
        CompletesDict = Komplety.ToDictionary(p => p.Order.orderID, q => q);
        await databaseAccessLayer.RefreshBodies(bodki.Select(p => p.itembodyID).ToList());
        List<Complete> noweOrdy = Komplety.Where(komplet => !stareOrdy.Any(ord => komplet.Order.orderID == ord.Key && komplet.Order.status == ord.Value)).ToList();
        OnCompletesChanged(noweOrdy.Select(p=>p.Order.orderID).ToList());

    }


    public void HandleCompleteChange(List<Complete> changedData)
    {
        if (Komplety == null) { return; }
        var ids = new List<int>();
        for (int i = 0; i < Komplety.Count(); i++)
        {
            var changedItem = changedData.FirstOrDefault(p => p.Order.orderID == Komplety[i].Order.orderID);
            if (changedItem != null)
            {
                Komplety[i] = changedItem;
                ids.Add(changedItem.Order.orderID);
            }
        }
        if (ids.Count > 0)
        {
            //         CompletesChanged?.Invoke(ids);
            OnCompletesChanged(ids);
        }
    }

    public async Task OrdersNeedRefreshing(List<Complete> completes)
    {
            
        if (Komplety==null) { return; }
        for (int i = 0; i < Komplety.Count(); i++)
        {
            // Sprawdzanie, czy element z listy Komplety pasuje do któregokolwiek elementu z listy kompy
            if (completes.Any(k => k.Order.orderID == Komplety[i].Order.orderID))
            {
                // Znalezienie pasującego elementu z kompy
                var matchingKomplet = completes.First(k => k.Order.orderID == Komplety[i].Order.orderID);

                // Zastąpienie elementu w liście Komplety
                Komplety[i] = matchingKomplet;

                // Aktualizacja słownika
                CompletesDict[matchingKomplet.Order.orderID] = matchingKomplet;
            }
        }
        OnCompletesChanged(completes.Select(p=>p.Order.orderID).ToList());
    }




}