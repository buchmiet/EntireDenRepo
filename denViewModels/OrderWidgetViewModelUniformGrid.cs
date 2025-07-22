using CommunityToolkit.Mvvm.ComponentModel;
using denModels;

namespace denViewModels;

public class OrderWidgetViewModelUniformGrid : ObservableObject, IOrderWidgetViewModel
{
    private string _boldText;
    private string _regularText;

    public string BoldText
    {
        get => _boldText;
        set => SetProperty(ref _boldText, value);
    }

    public string RegularText
    {
        get => _regularText;
        set => SetProperty(ref _regularText, value);
    }
    public OrderWidgetViewModelUniformGrid(string boldText, string regularText)
    {
        _boldText = boldText;
        _regularText = regularText;
    }
}