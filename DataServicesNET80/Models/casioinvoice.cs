namespace DataServicesNET80.Models;

public partial class casioinvoice
{
    public int CasioInvoiceId { get; set; }

    public DateTime date { get; set; }

    public string mpn { get; set; } = null!;

    public decimal? price { get; set; }

    public int quantity { get; set; }
}
