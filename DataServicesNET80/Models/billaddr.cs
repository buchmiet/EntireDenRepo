namespace DataServicesNET80.Models;

public partial class billaddr
{
    public int billaddrID { get; set; }

    public string? Line1 { get; set; }

    public string? Line2 { get; set; }

    public string? City { get; set; }

    public string CountryCode { get; set; } = null!;

    public string? CountrySubDivisionCode { get; set; }

    public string PostalCode { get; set; } = null!;

    public string? AddressAsAString { get; set; }

    public virtual countrycode CountryCodeNavigation { get; set; } = null!;

    public virtual ICollection<customer> customers { get; set; } = new List<customer>();
}
