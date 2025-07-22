namespace DataServicesNET80.Models;

public partial class deliveryprice
{
    public int DeliveryPriceId { get; set; }

    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public decimal price { get; set; }
}
