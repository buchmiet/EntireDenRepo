namespace DataServicesNET80.Models;

public partial class asinsku
{
    public int asinskuID { get; set; }

    public string asin { get; set; } = null!;

    public string sku { get; set; } = null!;

    public int locationID { get; set; }

    public virtual location location { get; set; } = null!;
}
