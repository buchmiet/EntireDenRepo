namespace DataServicesNET80.Models;

public partial class itembody
{
    public int itembodyID { get; set; }

    public int brandID { get; set; }

    public string name { get; set; } = null!;

    public string myname { get; set; } = null!;

    public string mpn { get; set; } = null!;

    public bool visible { get; set; }

    public string? description { get; set; }

    public bool readyTotrack { get; set; }

    public int typeId { get; set; }

    public string? logoPic { get; set; }

    public string? packagePic { get; set; }

    public string? fullsearchterm { get; set; }

    public int weight { get; set; }

    public virtual ICollection<bodiesgrouped> bodiesgroupeds { get; set; } = new List<bodiesgrouped>();

    public virtual ICollection<bodyinthebox> bodyintheboxes { get; set; } = new List<bodyinthebox>();

    public virtual brand brand { get; set; } = null!;

    public virtual ICollection<itemheader> itemheaders { get; set; } = new List<itemheader>();

    public virtual ICollection<itmitmassociation> itmitmassociationsourceBodyNavigations { get; set; } = new List<itmitmassociation>();

    public virtual ICollection<itmitmassociation> itmitmassociationtargetBodyNavigations { get; set; } = new List<itmitmassociation>();

    public virtual ICollection<itmmarketassoc> itmmarketassocitembodies { get; set; } = new List<itmmarketassoc>();

    public virtual ICollection<itmmarketassoc> itmmarketassocsoldWithNavigations { get; set; } = new List<itmmarketassoc>();

    public virtual ICollection<itmparameter> itmparameters { get; set; } = new List<itmparameter>();

    public virtual ICollection<logevent> logevents { get; set; } = new List<logevent>();

    public virtual ICollection<orderitem> orderitems { get; set; } = new List<orderitem>();

    public virtual ICollection<part2itemass> part2itemasses { get; set; } = new List<part2itemass>();

    public virtual ICollection<photo> photos { get; set; } = new List<photo>();

    public virtual shopitem? shopitem { get; set; }

    public virtual ICollection<stockshot> stockshots { get; set; } = new List<stockshot>();

    public virtual type type { get; set; } = null!;
}
