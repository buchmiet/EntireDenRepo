namespace DataServicesNET80.Models;

public partial class parameteranditsvalue
{
    public int parameterID { get; set; }

    public string parameterName { get; set; } = null!;

    public int parameterValueID { get; set; }

    public string valueName { get; set; } = null!;

    public int pos { get; set; }
}
