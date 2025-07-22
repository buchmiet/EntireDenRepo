namespace DataServicesNET80.Models;

public partial class searchentry
{
    public int searchentryId { get; set; }

    public string searchPhrase { get; set; } = null!;

    public DateTime searchTimeStamp { get; set; }
}
