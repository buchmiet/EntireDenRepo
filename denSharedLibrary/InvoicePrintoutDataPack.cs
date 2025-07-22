using DataServicesNET80;
using DataServicesNET80.Models;

namespace denSharedLibrary;

public class InvoicePrintoutDataPack
{
    //  public Dictionary<int, double> kursy;
    public Dictionary<string,string> postageTypes;
    public Dictionary<string, string> kantry;
    public Dictionary<int, string> Brands;
    public Dictionary<int, string> types;
    public Dictionary<int, List<parametervalue>> cechyValues;
    public Dictionary<int, string> markety;
    public Dictionary<int, currency> currencies;
    public Dictionary<int, AssociatedData> items;
    public Dictionary<int, decimal> VatRates;
    public int kolorId;       
}