using CommunityToolkit.Mvvm.ComponentModel;
using denSharedLibrary;

namespace denViewModels;

public class FilterEventArgs : EventArgs
{
    public List<int> Suppliers;
    public List<int> Markets;
    public List<int> Types;
}

public class FilterProductsViewModel : ObservableObject
{

       

    public event EventHandler<FilterEventArgs> FiltersChanged;

    protected virtual void OnFiltersChanged(List<int> suppliers, List<int> markets, List<int> types)
    {
        FiltersChanged?.Invoke(this, new FilterEventArgs { Suppliers = suppliers, Markets = markets, Types = types });
    }


    public RemovableButtonsInWrapPanelViewModel TypesViewModel { get; set; }
    public RemovableButtonsInWrapPanelViewModel OnMarketsViewModel { get; set; }
    public RemovableButtonsInWrapPanelViewModel NotOnMarketsViewModel { get; set; }
    public RemovableButtonsInWrapPanelViewModel FromSuppliersViewModel { get; set; }
    public RemovableButtonsInWrapPanelViewModel NotFromSuppliersViewModel { get; set; }

    public FilterProductsViewModel(Dictionary<int, string> _suppliers, Dictionary<int, string> _markets, Dictionary<int, string> _types, IDialogService dialogService)
    {
        TypesViewModel = new RemovableButtonsInWrapPanelViewModel("Choose product type", _types, dialogService, "Add Type");
        TypesViewModel.NegFilterProductsEvent += TypesCollectionChanged;
        OnMarketsViewModel = new RemovableButtonsInWrapPanelViewModel("Sold on Markets", _markets);
        OnMarketsViewModel.FilterProductsEvent += OnMarketsItemRemoved;
        NotOnMarketsViewModel = new RemovableButtonsInWrapPanelViewModel("Not sold on markets", new Dictionary<int, string>());
        NotOnMarketsViewModel.FilterProductsEvent += NotOnMarketsItemRemoved;
        FromSuppliersViewModel = new RemovableButtonsInWrapPanelViewModel("From suppliers", _suppliers);
        FromSuppliersViewModel.FilterProductsEvent += FromSuppliersItemRemoved;
        NotFromSuppliersViewModel = new RemovableButtonsInWrapPanelViewModel("Not from suppliers", new Dictionary<int, string>());
        NotFromSuppliersViewModel.FilterProductsEvent += NotFromSuppliersItemRemoved;

    }

    public void OnMarketsItemRemoved(int id, string tekst)
    {
        NotOnMarketsViewModel.AddItem(id, tekst);


        OnFiltersChanged(
            FromSuppliersViewModel.ItemsCollection.Select(p => p.Id).ToList(),
            OnMarketsViewModel.ItemsCollection.Select(p => p.Id).ToList(),
            TypesViewModel.ItemsCollection.Count == 0 ? TypesViewModel.Collection.Select(p => p.Key).ToList() : TypesViewModel.ItemsCollection.Select(p => p.Id).ToList()
        );
    }
    public void NotOnMarketsItemRemoved(int id, string tekst)
    {
        OnMarketsViewModel.AddItem(id, tekst);
    }

    public void FromSuppliersItemRemoved(int id, string tekst)
    {
        NotFromSuppliersViewModel.AddItem(id, tekst);
        OnFiltersChanged(
            FromSuppliersViewModel.ItemsCollection.Select(p => p.Id).ToList(),
            OnMarketsViewModel.ItemsCollection.Select(p => p.Id).ToList(),
            TypesViewModel.ItemsCollection.Count == 0 ? TypesViewModel.Collection.Select(p => p.Key).ToList() : TypesViewModel.ItemsCollection.Select(p => p.Id).ToList()
        );
    }
    public void NotFromSuppliersItemRemoved(int id, string tekst)
    {
        FromSuppliersViewModel.AddItem(id, tekst);
        OnFiltersChanged(
            FromSuppliersViewModel.ItemsCollection.Select(p => p.Id).ToList(),
            OnMarketsViewModel.ItemsCollection.Select(p => p.Id).ToList(),
            TypesViewModel.ItemsCollection.Count == 0 ? TypesViewModel.Collection.Select(p => p.Key).ToList() : TypesViewModel.ItemsCollection.Select(p => p.Id).ToList()
        );
    }

    public void TypesCollectionChanged()
    {
        OnFiltersChanged(
            FromSuppliersViewModel.ItemsCollection.Select(p => p.Id).ToList(),
            OnMarketsViewModel.ItemsCollection.Select(p => p.Id).ToList(),
            TypesViewModel.ItemsCollection.Count == 0 ? TypesViewModel.Collection.Select(p => p.Key).ToList() : TypesViewModel.ItemsCollection.Select(p => p.Id).ToList()
        );
    }

}