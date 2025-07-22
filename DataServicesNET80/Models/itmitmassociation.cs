namespace DataServicesNET80.Models;

public partial class itmitmassociation
{
    public int itmitmassID { get; set; }

    public int sourceBody { get; set; }

    public int targetBody { get; set; }

    public virtual itembody sourceBodyNavigation { get; set; } = null!;

    public virtual itembody targetBodyNavigation { get; set; } = null!;
}
