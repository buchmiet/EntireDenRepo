namespace DataServicesNET80.Models;

public partial class itemitsparametersandvalue
{
    public int itembodyID { get; set; }

    public int typeID { get; set; }

    public int parameterID { get; set; }

    public string? ParameterName { get; set; }

    public int? parameterValueID { get; set; }

    public string? ParameterValueName { get; set; }

    public int? pos { get; set; }
}
