namespace DataServicesNET80.Models;

public partial class brand
{
    public int brandID { get; set; }

    public string? name { get; set; }

    public virtual ICollection<itembody> itembodies { get; set; } = new List<itembody>();
}
