using DataServicesNET80.Models;

namespace DataServicesNET80;

public class Body2Add
{
    public itembody body { get; set; }
    public decimal price { get; set; }
    public int quantity { get; set; }
    public DateTime when { get; set; }
    public int vatrate { get; set; }
    public decimal xchgrate { get; set; }
    public string purchasecurrency { get; set; }
    public string acquiredcurrency { get; set; }

}