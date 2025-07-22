using denModels;
using denSharedLibrary;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace denViewModels.CurrentOrders;

public class ChangeStatusViewModel : ObservableObject, IAsyncDialogViewModel
{
    public event AsyncEventHandler RequestClose;
    private ObservableCollection<CSViewItem> _itemList;
    public ObservableCollection<CSViewItem> ItemList
    {
        get => _itemList;
        set => SetProperty(ref _itemList, value);
    }

    private KeyValuePair<string, string> _selectedStatus;
    public KeyValuePair<string, string> SelectedStatus
    {
        get => _selectedStatus;
        set => SetProperty(ref _selectedStatus, value);
    }

    public List<orderStatusUpdateModel> Result { get; set; }


    public List<orderStatusUpdateModel> GetResult()
    {


        var zwrotka = new List<orderStatusUpdateModel>();
        foreach (CSViewItem hh in ItemList)
        {
            zwrotka.Add(new orderStatusUpdateModel
            {
                Id = hh.orderId,
                status = SelectedStatus.Key,
                tracking = hh.Tracking
            });
        }
        return zwrotka;

    }


    public Dictionary<string, string> StatusChoices { get; set; }

    public ICommand UpdateClickCommand { get; set; }
    public ICommand ShipDispCommand { get; set; }
    public ICommand CancelClickCommand { get; set; }

    IDialogService _dialogService;

    public ChangeStatusViewModel(List<StatusChangeViewItem> orderki, Dictionary<string, string> wybory, IDialogService dialogService)
    {
        _dialogService = dialogService;
        ItemList = new ObservableCollection<CSViewItem>();
        int i = 1;
        foreach (var ko in orderki)
        {
            ItemList.Add(new CSViewItem
            {
                Number = i,
                Tracking = ko.Tracking,
                Buyer = ko.Buyer,
                Country = ko.Country,
                Market = ko.Market,
                PostalCode = ko.PostalCode,
                Status = ko.Status,
                orderId = ko.orderId,
                Product = ko.Product
            });

            i++;
        }


        StatusChoices = wybory;
        SelectedStatus = StatusChoices.First();
        UpdateClickCommand = new AsyncRelayCommand(UpdateClick);
        ShipDispCommand = new RelayCommand(ShipDisp);
        CancelClickCommand = new RelayCommand(CancelClick);
    }

    private async Task UpdateClick()
    {
        if (SelectedStatus.Key.Equals("NONE"))
        {
            await _dialogService.ShowMessage("Info", "Please do choose the new status of selected orders that you wish to change.");

        }
        else
        {
            Result = GetResult();
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ShipDisp()
    {
        var zwrotka = new List<orderStatusUpdateModel>();
        foreach (CSViewItem hh in ItemList)
        {
            zwrotka.Add(new orderStatusUpdateModel
            {
                Id = hh.orderId,
                status = "SHIP",
                tracking = hh.Tracking
            });
        }
        Result = zwrotka;
        RequestClose?.Invoke(this, EventArgs.Empty);
    }



    private void CancelClick()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);

    }


}