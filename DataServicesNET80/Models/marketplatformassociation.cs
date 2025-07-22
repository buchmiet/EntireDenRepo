namespace DataServicesNET80.Models;

public partial class marketplatformassociation
{
    public int marketID { get; set; }

    public int platformID { get; set; }

    public int MarketPlatformAssociationID { get; set; }

    public virtual market market { get; set; } = null!;

    public virtual platform platform { get; set; } = null!;
}
