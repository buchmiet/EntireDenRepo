using DataServicesNET80;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using DataServicesNET80.Models;
using DataServicesNET80.DatabaseAccessLayer;

namespace denViewModels;

public class AssociatedMarketsViewModel : ObservableObject
{
    // Nazwa cechy, która zostanie wyświetlona jako Label
    public string MarketName { get; set; }
    public int Id { get; set; }
    public ICommand EditCommand { get; set; }

    public ICommand RemoveCommand { get; set; }
    private FullItmMarketAss _fu;
    // Lista dostępnych wartości cech do wyboru w ComboBox

    // Wybrana wartość cechy
    private itmmarketassoc _associatedMarket;
    public itmmarketassoc AssociatedMarket
    {
        get { return _associatedMarket; }
        set
        {

            _associatedMarket = value;
            MarketName = _databaseAccessLayer.markety().Result[_associatedMarket.marketID].name;
            Id = _associatedMarket.itmmarketassID;
            OnPropertyChanged(nameof(AssociatedMarket));
        }
    }
    public IDatabaseAccessLayer _databaseAccessLayer;
    public AssociatedMarketsViewModel(FullItmMarketAss fuu, IDatabaseAccessLayer databaseService)
    {
        _fu = fuu;
        _databaseAccessLayer = databaseService;
        AssociatedMarket = _fu.itmmarketassoc;
    }


}