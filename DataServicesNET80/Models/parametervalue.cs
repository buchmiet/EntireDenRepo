namespace DataServicesNET80.Models;

public partial class parametervalue
{
    public int parameterValueID { get; set; }

    public int parameterID { get; set; }

    public string name { get; set; } = null!;

    public int pos { get; set; }

    public virtual ICollection<colourtranslation> colourtranslations { get; set; } = new List<colourtranslation>();

    public virtual ICollection<itmparameter> itmparameters { get; set; } = new List<itmparameter>();

    public virtual parameter parameter { get; set; } = null!;
}
