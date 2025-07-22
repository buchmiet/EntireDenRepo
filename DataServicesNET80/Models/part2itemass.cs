namespace DataServicesNET80.Models;

public partial class part2itemass
{
    public int part2itemassID { get; set; }

    public int itembodyID { get; set; }

    public int watchID { get; set; }

    public string watchsearchterm { get; set; } = null!;

    public virtual itembody itembody { get; set; } = null!;

    public virtual watch watch { get; set; } = null!;
}
