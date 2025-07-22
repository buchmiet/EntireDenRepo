using System.Collections.ObjectModel;

namespace denViewModels.ProductBrowser;

public class SimpleDescription
{
    public ObservableCollection<IBaseFieldViewModel> Items { get; set; } = [];
}