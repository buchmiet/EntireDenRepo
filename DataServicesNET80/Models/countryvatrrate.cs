namespace DataServicesNET80.Models;

public partial class countryvatrrate
{
    public int Countryvatrateid { get; set; }

    public string code { get; set; } = null!;

    public decimal rate { get; set; }

    public virtual countrycode codeNavigation { get; set; } = null!;
}
