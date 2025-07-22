namespace DataServicesNET80.Models;

public partial class multidrawer
{
    public int MultiDrawerID { get; set; }

    public int rows { get; set; }

    public int columns { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<bodyinthebox> bodyintheboxes { get; set; } = new List<bodyinthebox>();
}
