namespace DataServicesNET80.Models;

public partial class itmparameter
{
    public int itmparameterID { get; set; }

    public int itembodyID { get; set; }

    public int parameterID { get; set; }

    public int parameterValueID { get; set; }

    public virtual itembody itembody { get; set; } = null!;

    public virtual parameter parameter { get; set; } = null!;

    public virtual parametervalue parameterValue { get; set; } = null!;
}
