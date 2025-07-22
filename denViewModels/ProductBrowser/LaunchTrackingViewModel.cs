using denModels;
using denSharedLibrary;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class LaunchTrackingViewModel : ObservableObject, IAsyncDialogViewModel
{



    public ICommand OkCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }


    public event AsyncEventHandler RequestClose;

    private ObservableCollection<ItemQuantity> _itemQuantities;
    public ObservableCollection<ItemQuantity> ItemQuantities
    {
        get => _itemQuantities;
        set => SetProperty(ref _itemQuantities, value);
    }


    public Dictionary<Idname, int> Result
    {
        get
        {

            return ItemQuantities.ToDictionary(iq => iq.Idname, iq => iq.Quantity);
        }
    }



    public LaunchTrackingViewModel(Dictionary<Idname, int> quantities)
    {


        OkCommand = new AsyncRelayCommand(Confirm);
        CancelCommand = new AsyncRelayCommand(Cancel);


        ItemQuantities = new ObservableCollection<ItemQuantity>();


        foreach (KeyValuePair<Idname, int> pair in quantities)
        {
            ItemQuantities.Add(new ItemQuantity
            {
                Idname = pair.Key,
                Quantity = pair.Value
            });
        }
    }


    private async Task Confirm()
    {

        RequestClose?.Invoke(this, EventArgs.Empty);
    }


    private async Task Cancel()
    {

        RequestClose?.Invoke(this, EventArgs.Empty);
    }


}