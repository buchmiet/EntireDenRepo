using DataServicesNET80.Models;

namespace DataServicesNET80;

public class AssociatedData
{
    public itembody itembody { get; set; }
    public List<itemheader> ItemHeaders { get; set; } = new List<itemheader>();
    public bodyinthebox bodyinthebox { get; set; }
    public List<itmparameter> ItmCechies { get; set; } = new List<itmparameter>();
    public List<itmmarketassoc> ItmMarketAssocs { get; set; } = new List<itmmarketassoc>();
    public Dictionary<parameter, parametervalue> PrzyporzadkowaniaCech { get; set; }
    public List<photo> Photos { get; set; }

}