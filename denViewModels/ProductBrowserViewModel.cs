using DataServicesNET80.DatabaseAccessLayer;
using denModels;
using denSharedLibrary;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class ProductBrowserViewModel : ObservableObject, IAsyncDialogViewModel
{
    public event AsyncEventHandler RequestClose;
    private ObservableCollection<Idname> _suppliers;
    private ObservableCollection<Idname> _brands;
    private ObservableCollection<Idname> _types;
    private ObservableCollection<productItem> _products = new();
    private Idname _selectedSupplier;
    private Idname _selectedBrand;
    private Idname _selectedType;
    private string _productName;
    private string _productMPN;

    public ICommand LoadDataCommand { get; }
    private RelayCommand<productItem> _doubleClickCommand;
    public RelayCommand<productItem> DoubleClickCommand
    {
        get => _doubleClickCommand;
        set => SetProperty(ref _doubleClickCommand, value);
    }

    public int Result { get; set; }

    public IDatabaseAccessLayer _databaseAccessLayer;
    public ProductBrowserViewModel(IDatabaseAccessLayer databaseAccessLayer)
    {
        _databaseAccessLayer = databaseAccessLayer;
        LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);

        SearchCommand = new RelayCommand(SearchCommandExecute);
        DoubleClickCommand = new RelayCommand<productItem>(OnRowDoubleClick);
    }

    bool IsDataLoaded = false;
    private async Task LoadDataAsync()
    {
        if (IsDataLoaded) return;
        Suppliers = new ObservableCollection<Idname>((await _databaseAccessLayer.Suppliers()).Select(header => new Idname { Id = header.Key, Name = header.Value.name }).ToList());
        SelectedSupplier = Suppliers.First();
        Brands = new ObservableCollection<Idname>((await _databaseAccessLayer.Brands()).Select(header => new Idname { Id = header.Key, Name = header.Value }).ToList());
        SelectedBrand = Brands.First();
        Types = new ObservableCollection<Idname>((await _databaseAccessLayer.types()).Select(header => new Idname { Id = header.Key, Name = header.Value }).ToList());
        SelectedType = Types.First();

        ShowProducts();
        IsDataLoaded = true;
    }

    private void OnRowDoubleClick(productItem product)
    {
        Result = product.id;// GetResult();
        RequestClose?.Invoke(this, EventArgs.Empty);
    }


    public void SearchCommandExecute()
    {
        ShowProducts();
    }

    public async void ShowProducts()
    {
        if (SelectedBrand == null || SelectedSupplier == null || SelectedType == null)
        {
            return;
        }
        var prodki = _databaseAccessLayer.items.Where(p => p.Value.itembody.brandID == SelectedBrand.Id && p.Value.itembody.typeId == SelectedType.Id && p.Value.ItemHeaders.Any(q => q.supplierID == SelectedSupplier.Id)).ToList();
        if (!string.IsNullOrEmpty(ProductMPN))
        {
            prodki = prodki.Where(p => p.Value.itembody.mpn.Contains(ProductMPN)).ToList();
        }
        if (!string.IsNullOrEmpty(ProductName))
        {
            prodki = prodki.Where(p => (p.Value.itembody.name.ToLower().Contains(ProductName.ToLower()))).ToList();
        }
        int i = 1;
        Products.Clear();
        foreach (var item in prodki)
        {
            Products.Add(new productItem
            {
                id = item.Key,
                Number = i,
                Quantity = item.Value.ItemHeaders.Sum(p => p.quantity),
                fullName = item.Value.itembody.name,
                mpn = item.Value.itembody.mpn,
                Readytotrack = item.Value.itembody.readyTotrack,
                myname = item.Value.itembody.myname,
                Brand = (await _databaseAccessLayer.Brands())[item.Value.itembody.brandID],
                Type = (await _databaseAccessLayer.types())[item.Value.itembody.typeId]
            });
            i++;
        }

    }

       
    public ObservableCollection<Idname> Suppliers
    {
        get => _suppliers;
        set => SetProperty(ref _suppliers, value);
    }

     
    public ObservableCollection<Idname> Brands
    {
        get => _brands;
        set => SetProperty(ref _brands, value);
    }

      
    public ObservableCollection<Idname> Types
    {
        get => _types;
        set => SetProperty(ref _types, value);
    }

   
    public ObservableCollection<productItem> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    public Idname SelectedSupplier
    {
        get => _selectedSupplier;
        set
        {
            if (SetProperty(ref _selectedSupplier, value))
            {
                ShowProducts();
            }
        }
    }

      
    public Idname SelectedType
    {
        get => _selectedType;
        set
        {
            if (SetProperty(ref _selectedType, value))
            {
                ShowProducts();
            }
        }
    }

    public Idname SelectedBrand
    {
        get => _selectedBrand;
        set
        {
            if (SetProperty(ref _selectedBrand, value))
            {
                ShowProducts();
            }
        }
    }

    public string ProductName
    {
        get => _productName;
        set => SetProperty(ref _productName, value);
    }

     
    public string ProductMPN
    {
        get => _productMPN;
        set => SetProperty(ref _productMPN, value);
    }

    public ICommand SearchCommand { get; set; }
    public ICommand ClearCommand { get; set; }


    private async Task Cancel()
    {
        Result = -1;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }


}