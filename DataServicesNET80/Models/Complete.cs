namespace DataServicesNET80.Models;

public class Complete
{
    public order Order { get; set; }
    public customer Customer { get; set; }
    public billaddr BillAddr { get; set; }
    public List<orderitem> OrderItems { get; set; }
}