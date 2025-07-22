namespace DataServicesNET80.Models;

public partial class shopitem
{
    public int shopitemId { get; set; }

    public decimal price { get; set; }

    public int itembodyID { get; set; }

    public int locationID { get; set; }

    public int quantity { get; set; }

    public string currencyCode { get; set; } = null!;

    public bool active { get; set; }

    public int? soldQuantity { get; set; }

    public virtual currency currencyCodeNavigation { get; set; } = null!;

    public virtual itembody itembody { get; set; } = null!;

    public virtual location location { get; set; } = null!;
}
