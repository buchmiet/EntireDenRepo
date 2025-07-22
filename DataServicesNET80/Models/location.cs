namespace DataServicesNET80.Models;

public partial class location
{
    public int locationID { get; set; }

    public string name { get; set; } = null!;

    public string? currency { get; set; }

    public bool? active { get; set; }

    public virtual ICollection<amazonmarketplace> amazonmarketplaces { get; set; } = new List<amazonmarketplace>();

    public virtual ICollection<asinsku> asinskus { get; set; } = new List<asinsku>();

    public virtual currency? currencyNavigation { get; set; }

    public virtual ICollection<itemheader> itemheaders { get; set; } = new List<itemheader>();

    public virtual ICollection<itmmarketassoc> itmmarketassocs { get; set; } = new List<itmmarketassoc>();

    public virtual ICollection<locmarassociation> locmarassociations { get; set; } = new List<locmarassociation>();

    public virtual ICollection<order> orders { get; set; } = new List<order>();

    public virtual ICollection<shopitem> shopitems { get; set; } = new List<shopitem>();

    public virtual ICollection<stockshot> stockshots { get; set; } = new List<stockshot>();
    public virtual ICollection<OauthToken> OauthTokens { get; set; }= new List<OauthToken>();
}
