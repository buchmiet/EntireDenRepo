namespace DataServicesNET80.Models;

public partial class colourtranslation
{
    public int Id { get; set; }

    public int kodKoloru { get; set; }

    public string schemat { get; set; } = null!;

    public int col1 { get; set; }

    public int? col2 { get; set; }

    public int? col3 { get; set; }

    public int? col4 { get; set; }

    public virtual parametervalue kodKoloruNavigation { get; set; } = null!;
}
