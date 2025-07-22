namespace DataServicesNET80.Models;

public partial class order
{
    public int orderID { get; set; }

    public bool quickbooked { get; set; }

    public int customerID { get; set; }

    public DateTime paidOn { get; set; }

    public DateTime dispatchedOn { get; set; }

    public string tracking { get; set; } = null!;

    public int market { get; set; }

    public int locationID { get; set; }

    public string? salecurrency { get; set; }

    public string? acquiredcurrency { get; set; }

    public decimal saletotal { get; set; }

    public decimal xchgrate { get; set; }

    public bool VAT { get; set; }

    public string? order_notes { get; set; }

    public string status { get; set; } = null!;

    public decimal postagePrice { get; set; }

    public string? postageType { get; set; }

    public int VATRateID { get; set; }

    public virtual vatrate VATRate { get; set; } = null!;

    public virtual currency? acquiredcurrencyNavigation { get; set; }

    public virtual customer customer { get; set; } = null!;

    public virtual ICollection<invoicetxn> invoicetxns { get; set; } = new List<invoicetxn>();

    public virtual location location { get; set; } = null!;

    public virtual market marketNavigation { get; set; } = null!;

    public virtual ICollection<orderitem> orderitems { get; set; } = new List<orderitem>();

    public virtual postagetype? postageTypeNavigation { get; set; }

    public virtual currency? salecurrencyNavigation { get; set; }

    public virtual orderstatus statusNavigation { get; set; } = null!;
}
