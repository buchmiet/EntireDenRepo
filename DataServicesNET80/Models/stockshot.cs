namespace DataServicesNET80.Models;

public partial class stockshot
{
    public int StockShotID { get; set; }

    public int bodyid { get; set; }

    public int quantity { get; set; }

    public DateTime date { get; set; }

    public int locationID { get; set; }

    public virtual itembody body { get; set; } = null!;

    public virtual location location { get; set; } = null!;
}
