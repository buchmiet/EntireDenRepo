namespace DataServicesNET80.Models;

public partial class countrycode
{
    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public virtual ICollection<billaddr> billaddrs { get; set; } = new List<billaddr>();

    public virtual country2rmass? country2rmass { get; set; }

    public virtual ICollection<countryvatrrate> countryvatrrates { get; set; } = new List<countryvatrrate>();
}
