namespace DataServicesNET80.Models;

public partial class bodiesgrouped
{
    public int bodiesGroupedID { get; set; }

    public int group4bodiesID { get; set; }

    public int itemBodyID { get; set; }

    public virtual group4body group4bodies { get; set; } = null!;

    public virtual itembody itemBody { get; set; } = null!;
}
