namespace DataServicesNET80.Models;

public partial class orderitemtype
{
    public int OrderItemTypeId { get; set; }

    public string name { get; set; } = null!;

    public virtual ICollection<orderitem> orderitems { get; set; } = new List<orderitem>();
}
