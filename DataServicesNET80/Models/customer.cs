namespace DataServicesNET80.Models;

public partial class customer
{
    public int customerID { get; set; }

    public string? Title { get; set; }

    public string? GivenName { get; set; }

    public string? MiddleName { get; set; }

    public string? FamilyName { get; set; }

    public string? CompanyName { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? DisplayName { get; set; }

    public string? currency { get; set; }

    public string? customer_notes { get; set; }

    public int? billaddrID { get; set; }

    public virtual billaddr? billaddr { get; set; }

    public virtual currency? currencyNavigation { get; set; }

    public virtual ICollection<order> orders { get; set; } = new List<order>();
}
