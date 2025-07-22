namespace DataServicesNET80.Models;

public partial class casioukbackorder
{
    public int casioUKbackorderId { get; set; }

    public string mpn { get; set; } = null!;

    public int quantity { get; set; }

    public DateTime orderedon { get; set; }
}
