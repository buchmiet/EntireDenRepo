namespace DataServicesNET80.Models;

public partial class parameter
{
    public int parameterID { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<itmparameter> itmparameters { get; set; } = new List<itmparameter>();

    public virtual ICollection<parametervalue> parametervalues { get; set; } = new List<parametervalue>();

    public virtual ICollection<typeparassociation> typeparassociations { get; set; } = new List<typeparassociation>();
}
