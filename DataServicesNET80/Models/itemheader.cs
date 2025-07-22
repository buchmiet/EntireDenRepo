namespace DataServicesNET80.Models;

public partial class itemheader
{
    public int itemheaderID { get; set; }

    public int locationID { get; set; }

    public DateTime? purchasedOn { get; set; }

    public int supplierID { get; set; }

    public int itembodyID { get; set; }

    public decimal pricePaid { get; set; }

    public string? purchasecurrency { get; set; }

    public string? acquiredcurrency { get; set; }

    public decimal xchgrate { get; set; }

    public int quantity { get; set; }

    public int VATRateID { get; set; }

    public virtual vatrate VATRate { get; set; } = null!;

    public virtual currency? acquiredcurrencyNavigation { get; set; }

    public virtual itembody itembody { get; set; } = null!;

    public virtual location location { get; set; } = null!;

    public virtual ICollection<logevent> logevents { get; set; } = new List<logevent>();

    public virtual currency? purchasecurrencyNavigation { get; set; }

    public virtual supplier supplier { get; set; } = null!;
}
