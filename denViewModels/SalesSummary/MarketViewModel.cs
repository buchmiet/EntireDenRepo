using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels.SalesSummary;

public class MarketViewModel:ObservableObject
{
    private bool _isChecked;
    private string _marketName;
    public List<int> id;
    public bool CanRemove { get; set; } = false;

    public class MarketViewModelProperties
    {
        public bool IsChecked { get; set; }
        public List<int> Id { get; set; }
        public string MarketName { get; set; }
    }


    public MarketViewModel(KeyValuePair<List<int>,string> market)
    {
        _marketName = market.Value;
        id= market.Key;
    }

    public bool IsChecked
    {
        get => _isChecked;
        set 
        {
            if (SetProperty(ref _isChecked, value))
            {
                if (CheckChangedCommand != null)
                {
                    CheckChangedCommand.Execute(new MarketViewModelProperties { Id = id, MarketName = MarketName, IsChecked = IsChecked });
                }
            }
        }
    }

    public string MarketName
    {
        get => _marketName;
        set => SetProperty(ref _marketName, value);
    }

    public RelayCommand RemoveCommand { get; set; }
    public RelayCommand<MarketViewModel.MarketViewModelProperties> CheckChangedCommand { get; set; }
}