namespace DataServicesNET80.Models;

public partial class platform
{
    public int platformID { get; set; }

    public string? name { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<invoicetxn> invoicetxns { get; set; } = new List<invoicetxn>();

    public virtual ICollection<marketplatformassociation> marketplatformassociations { get; set; } = new List<marketplatformassociation>();
}
