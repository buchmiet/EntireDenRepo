namespace DataServicesNET80.Models;

public partial class typeparassociation
{
    public int typeID { get; set; }

    public int parameterID { get; set; }

    public int pos { get; set; }

    public virtual parameter parameter { get; set; } = null!;

    public virtual type type { get; set; } = null!;
}
