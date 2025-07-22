namespace DataServicesNET80.Models;

public partial class logevent
{
    public int logeventID { get; set; }

    public DateTime happenedOn { get; set; }

    public int? itemHeaderID { get; set; }

    public string _event { get; set; } = null!;

    public int itemBodyID { get; set; }

    public int? marketID { get; set; }

    public virtual itembody itemBody { get; set; } = null!;

    public virtual itemheader? itemHeader { get; set; }

    public virtual market? market { get; set; }
}
