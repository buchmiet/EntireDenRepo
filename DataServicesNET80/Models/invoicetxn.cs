namespace DataServicesNET80.Models;

public partial class invoicetxn
{
    public int invoiceTXNID { get; set; }

    public int platformID { get; set; }

    public string? qbInvoiceId { get; set; }

    public string? platformTXN { get; set; }

    public int orderID { get; set; }

    public int marketID { get; set; }

    public virtual market market { get; set; } = null!;

    public virtual order order { get; set; } = null!;

    public virtual platform platform { get; set; } = null!;
}
