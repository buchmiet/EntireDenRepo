using DataServicesNET80.Models;


namespace denViewModels.ProductBrowser.Model;

public class BrandsSuppliersMarkets
{
    public Dictionary<int, string> Brands;
    public Dictionary<int, string> Suppliers;
    public Dictionary<int, market> Markets;
    public Dictionary<int, string> Types;
}