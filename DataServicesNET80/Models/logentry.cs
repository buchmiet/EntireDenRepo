namespace DataServicesNET80.Models;

public partial class logentry
{
    public int logentryId { get; set; }

    public string _event { get; set; } = null!;

    public DateTime eventdate { get; set; }
}
