using CommunityToolkit.Mvvm.ComponentModel;
using denModels;

namespace denViewModels;

public class OrderWidgetViewModelTextBlock : ObservableObject, IOrderWidgetViewModel
{
    private string _text;

    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }
    public OrderWidgetViewModelTextBlock(string text)
    {
        _text = text;
    }
}