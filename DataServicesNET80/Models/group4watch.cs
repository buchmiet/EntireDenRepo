namespace DataServicesNET80.Models;

public partial class group4watch
{
    public int group4watchesID { get; set; }

    public string? name { get; set; }

    public virtual ICollection<mayalsofit> mayalsofits { get; set; } = new List<mayalsofit>();

    public virtual ICollection<watchesgrouped> watchesgroupeds { get; set; } = new List<watchesgrouped>();
}
