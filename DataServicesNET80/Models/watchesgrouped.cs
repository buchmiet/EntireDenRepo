namespace DataServicesNET80.Models;

public partial class watchesgrouped
{
    public int watchesGroupedID { get; set; }

    public int group4watchesID { get; set; }

    public int watchID { get; set; }

    public virtual group4watch group4watches { get; set; } = null!;

    public virtual watch watch { get; set; } = null!;
}
