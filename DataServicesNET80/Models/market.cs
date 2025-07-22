namespace DataServicesNET80.Models;

public partial class market
{
    public int marketID { get; set; }

    public string? name { get; set; }

    public bool IsInUse { get; set; }

    public string currency { get; set; } = null!;

    public virtual ICollection<amazonmarketplace> amazonmarketplaces { get; set; } = new List<amazonmarketplace>();

    public virtual currency currencyNavigation { get; set; } = null!;

    public virtual ICollection<invoicetxn> invoicetxns { get; set; } = new List<invoicetxn>();

    public virtual ICollection<itmmarketassoc> itmmarketassocs { get; set; } = new List<itmmarketassoc>();

    public virtual ICollection<locmarassociation> locmarassociations { get; set; } = new List<locmarassociation>();

    public virtual ICollection<logevent> logevents { get; set; } = new List<logevent>();

    public virtual ICollection<marketplatformassociation> marketplatformassociations { get; set; } = new List<marketplatformassociation>();

    public virtual ICollection<order> orders { get; set; } = new List<order>();
}
