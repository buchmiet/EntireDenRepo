namespace DataServicesNET80.Models;

public partial class itmmarketassoc
{
    public int itmmarketassID { get; set; }

    public int itembodyID { get; set; }

    public int marketID { get; set; }

    public int quantitySold { get; set; }

    public int? soldWith { get; set; }

    public int locationID { get; set; }

    public string itemNumber { get; set; } = null!;

    public string? SEName { get; set; }

    public virtual itembody itembody { get; set; } = null!;

    public virtual location location { get; set; } = null!;

    public virtual market market { get; set; } = null!;

    public virtual ICollection<orderitem> orderitems { get; set; } = new List<orderitem>();

    public virtual itembody? soldWithNavigation { get; set; }
}
