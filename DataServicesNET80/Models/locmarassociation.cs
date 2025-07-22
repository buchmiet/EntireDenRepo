namespace DataServicesNET80.Models;

public partial class locmarassociation
{
    public int loc { get; set; }

    public int reference { get; set; }

    public int pos { get; set; }

    public virtual location locNavigation { get; set; } = null!;

    public virtual market referenceNavigation { get; set; } = null!;
}
