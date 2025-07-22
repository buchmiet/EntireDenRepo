namespace DataServicesNET80.Models;

public partial class orderitem
{
    public int OrderItemId { get; set; }

    public string itemName { get; set; } = null!;

    public int quantity { get; set; }

    public int itembodyID { get; set; }

    public int OrderItemTypeId { get; set; }

    public decimal price { get; set; }

    public int? orderID { get; set; }

    public int? itmMarketAssID { get; set; }

    public int? ItemWeight { get; set; }

    public virtual orderitemtype OrderItemType { get; set; } = null!;

    public virtual itembody itembody { get; set; } = null!;

    public virtual itmmarketassoc? itmMarketAss { get; set; }

    public virtual order? order { get; set; }
}
