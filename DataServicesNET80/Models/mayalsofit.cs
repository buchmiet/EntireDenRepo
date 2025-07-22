namespace DataServicesNET80.Models;

public partial class mayalsofit
{
    public int mayalsofitID { get; set; }

    public int group4bodiesID { get; set; }

    public int group4watchesID { get; set; }

    public virtual group4body group4bodies { get; set; } = null!;

    public virtual group4watch group4watches { get; set; } = null!;
}
