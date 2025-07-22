namespace DataServicesNET80.Models;

public partial class keyvalue
{
    public int keyvalueID { get; set; }

    public string key { get; set; } = null!;

    public string value { get; set; } = null!;

    public DateTime timestamp { get; set; }
}
