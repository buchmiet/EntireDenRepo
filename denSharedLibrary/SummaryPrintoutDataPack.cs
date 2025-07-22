using DataServicesNET80;
using DataServicesNET80.Models;

namespace denSharedLibrary;

public class SummaryPrintoutDataPack
{
    public Dictionary<int, AssociatedData> items;
    public List<multidrawer> MultiDrawer;
    public Dictionary<int, string> OrderIdsPlatformTXNs;
    public Dictionary<int, string> Markety;
    public Dictionary<int, Dictionary<int, int>> OrderItemQuantitiesSold;
    public Dictionary <int,Dictionary<int,int>> SoldWiths;
}