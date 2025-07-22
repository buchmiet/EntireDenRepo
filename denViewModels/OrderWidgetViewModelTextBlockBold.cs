using CommunityToolkit.Mvvm.ComponentModel;
using denModels;

namespace denViewModels;

public class OrderWidgetViewModelTextBlockBold : ObservableObject, IOrderWidgetViewModel
{
    private string _text;

    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }
    public OrderWidgetViewModelTextBlockBold(string text)
    {
        _text = text;
    }
}