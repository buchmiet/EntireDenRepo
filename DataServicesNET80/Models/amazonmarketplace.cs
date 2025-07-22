namespace DataServicesNET80.Models;

public partial class amazonmarketplace
{
    public int Id { get; set; }

    public int locationID { get; set; }

    public int marketID { get; set; }

    public string code { get; set; } = null!;

    public virtual location location { get; set; } = null!;

    public virtual market market { get; set; } = null!;
}
