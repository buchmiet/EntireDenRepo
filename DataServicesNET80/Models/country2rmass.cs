namespace DataServicesNET80.Models;

public partial class country2rmass
{
    public int Country2RMAssId { get; set; }

    public string code { get; set; } = null!;

    public int RMZoneID { get; set; }

    public virtual countrycode codeNavigation { get; set; } = null!;
}
