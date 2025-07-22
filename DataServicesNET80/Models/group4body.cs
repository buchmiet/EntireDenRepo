namespace DataServicesNET80.Models;

public partial class group4body
{
    public int group4bodiesID { get; set; }

    public string? name { get; set; }

    public virtual ICollection<bodiesgrouped> bodiesgroupeds { get; set; } = new List<bodiesgrouped>();

    public virtual ICollection<mayalsofit> mayalsofits { get; set; } = new List<mayalsofit>();
}
