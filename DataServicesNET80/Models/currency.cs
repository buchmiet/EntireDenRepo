namespace DataServicesNET80.Models;

public partial class currency
{
    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public string symbol { get; set; } = null!;

    public virtual ICollection<customer> customers { get; set; } = new List<customer>();

    public virtual ICollection<itemheader> itemheaderacquiredcurrencyNavigations { get; set; } = new List<itemheader>();

    public virtual ICollection<itemheader> itemheaderpurchasecurrencyNavigations { get; set; } = new List<itemheader>();

    public virtual ICollection<location> locations { get; set; } = new List<location>();

    public virtual ICollection<market> markets { get; set; } = new List<market>();

    public virtual ICollection<order> orderacquiredcurrencyNavigations { get; set; } = new List<order>();

    public virtual ICollection<order> ordersalecurrencyNavigations { get; set; } = new List<order>();

    public virtual ICollection<shopitem> shopitems { get; set; } = new List<shopitem>();

    public virtual ICollection<supplier> suppliers { get; set; } = new List<supplier>();

    public virtual ICollection<xrate> xrateSourceCurrencyCodeNavigations { get; set; } = new List<xrate>();

    public virtual ICollection<xrate> xratecodeNavigations { get; set; } = new List<xrate>();
}
