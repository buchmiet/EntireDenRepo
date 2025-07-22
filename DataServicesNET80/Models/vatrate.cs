namespace DataServicesNET80.Models;

public partial class vatrate
{
    public int VATRateID { get; set; }

    public decimal Rate { get; set; }

    public string VATDescription { get; set; } = null!;

    public virtual ICollection<itemheader> itemheaders { get; set; } = new List<itemheader>();

    public virtual ICollection<order> orders { get; set; } = new List<order>();
}
