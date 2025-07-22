namespace DataServicesNET80.Models;

public partial class type
{
    public int typeID { get; set; }

    public string? name { get; set; }

    public virtual ICollection<itembody> itembodies { get; set; } = new List<itembody>();

    public virtual ICollection<typeparassociation> typeparassociations { get; set; } = new List<typeparassociation>();
}
