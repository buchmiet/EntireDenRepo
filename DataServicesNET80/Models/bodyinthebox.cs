namespace DataServicesNET80.Models;

public partial class bodyinthebox
{
    public int BodyInTheBoxID { get; set; }

    public int itembodyID { get; set; }

    public int MultiDrawerID { get; set; }

    public int row { get; set; }

    public int column { get; set; }

    public virtual multidrawer MultiDrawer { get; set; } = null!;

    public virtual itembody itembody { get; set; } = null!;
}
