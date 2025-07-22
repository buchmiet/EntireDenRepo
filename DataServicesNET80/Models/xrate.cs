namespace DataServicesNET80.Models;

public partial class xrate
{
    public int XrateId { get; set; }

    public DateTime date { get; set; }

    public decimal rate { get; set; }

    public string code { get; set; } = null!;

    public string SourceCurrencyCode { get; set; } = null!;

    public virtual currency SourceCurrencyCodeNavigation { get; set; } = null!;

    public virtual currency codeNavigation { get; set; } = null!;
}
