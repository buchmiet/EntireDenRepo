namespace DataServicesNET80.Models;

public partial class watch
{
    public int watchID { get; set; }

    public string name { get; set; } = null!;

    public bool haspic { get; set; }

    public string searchterm { get; set; } = null!;

    public string? watchCode { get; set; }

    public virtual ICollection<part2itemass> part2itemasses { get; set; } = new List<part2itemass>();

    public virtual ICollection<watchesgrouped> watchesgroupeds { get; set; } = new List<watchesgrouped>();
}
