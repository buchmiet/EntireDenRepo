namespace DataServicesNET80.Models;

public partial class supplier
{
    public int supplierID { get; set; }

    public string? name { get; set; }

    public string currency { get; set; } = null!;

    public virtual currency currencyNavigation { get; set; } = null!;

    public virtual ICollection<itemheader> itemheaders { get; set; } = new List<itemheader>();
}
