namespace DataServicesNET80.Models;

public partial class photo
{
    public int photoID { get; set; }

    public string path { get; set; } = null!;

    public int itembodyID { get; set; }

    public int pos { get; set; }

    public virtual itembody itembody { get; set; } = null!;
}
