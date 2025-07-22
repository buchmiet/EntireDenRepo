namespace DataServicesNET80.Models;

public partial class zibiinvoice
{
    public int zibiinvoiceId { get; set; }

    public string mpn { get; set; } = null!;

    public string? name { get; set; }

    public decimal price { get; set; }

    public int discount { get; set; }

    public decimal priceAfterD { get; set; }

    public decimal vat { get; set; }

    public DateTime purchasedOn { get; set; }

    public int quantity { get; set; }
}
