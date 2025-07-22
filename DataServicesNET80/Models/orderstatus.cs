namespace DataServicesNET80.Models;

public partial class orderstatus
{
    public string code { get; set; } = null!;

    public string name { get; set; } = null!;

    public virtual ICollection<order> orders { get; set; } = new List<order>();
}
