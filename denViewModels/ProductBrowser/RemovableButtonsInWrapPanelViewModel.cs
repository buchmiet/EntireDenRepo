using denSharedLibrary;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace denViewModels;

public class RemovableButtonsInWrapPanelViewModel : ObservableObject
{


    public delegate void CollectionChangedEventHandler(int id, string item);
    public event CollectionChangedEventHandler FilterProductsEvent;

    public delegate void NegCollectionChangedEventHandler();
    public event NegCollectionChangedEventHandler NegFilterProductsEvent;


    private string _description;
    public string Description
    {
        get => _description;
        set { SetProperty(ref _description, value); }
    }


    private string _buttonText;
    public string ButtonText
    {
        get => _buttonText;
        set { SetProperty(ref _buttonText, value); }
    }

    private bool _beingDisplayed = false;
    public bool BeingDisplayed
    {
        get => _beingDisplayed;
        set { SetProperty(ref _beingDisplayed, value); }
    }

    private bool _isButtonVisible = false;
    public bool IsButtonVisible
    {
        get => _isButtonVisible;
        set { SetProperty(ref _isButtonVisible, value); }
    }


    private bool _isButtonEnabled = true;
    public bool IsButtonEnabled
    {
        get => _isButtonEnabled;
        set { SetProperty(ref _isButtonEnabled, value); }
    }

    public AsyncRelayCommand OpenSelectionWindowCommand { get; set; }


    public ObservableCollection<LabelViewModel> ItemsCollection { get; set; } = new ObservableCollection<LabelViewModel>();
    public Dictionary<int, string> Collection;
    IDialogService _dialogService;

    public RemovableButtonsInWrapPanelViewModel(string description, Dictionary<int, string> collection, IDialogService? dialogService = null, string? _buttonText = null)
    {
        Description = description;

        if (_buttonText == null)
        {
            foreach (var item in collection)
            {
                ItemsCollection.Add(new LabelViewModel
                    {
                        Id = item.Key,
                        Tekst = item.Value,
                        RemoveCommand = new RelayCommand(() => { RemoveItem(item.Key); })
                    }
                );
            }
            if (ItemsCollection.Count > 0)
            {
                BeingDisplayed = true;
            }

        }
        else
        {
            ButtonText = _buttonText;
            OpenSelectionWindowCommand = new AsyncRelayCommand(OpenSelectionWindowExecute);
            Collection = collection;
            _dialogService = dialogService;
            BeingDisplayed = true;
            IsButtonVisible = true;
        }
    }

    public bool CanOpenSelectionWindowExecute()
    {
        if (ItemsCollection.Count == Collection.Count) { return false; }
        return true;
    }


    public async Task OpenSelectionWindowExecute()
    {
        var typki = Collection.Where(p => !ItemsCollection.Select(q => q.Id).Any(z => z == p.Key));
        var addNewType = new AddTypesViewModel(typki.ToDictionary(p => p.Key, q => q.Value));
        addNewType.RequestClose += async (sender, e) =>
        {
            var result = ((AddTypesViewModel)sender).Response;
            if (result != null)
            {
                foreach (var item in result)
                {

                    var element = new LabelViewModel
                    {
                        Id = item,
                        Tekst = Collection[item],
                        RemoveCommand = new RelayCommand(() => { RemoveItem(item); })
                    };

                    int indexToInsert = FindIndexToInsert(element);
                    ItemsCollection.Insert(indexToInsert, element);
                    NegFilterProductsEvent?.Invoke();
                    IsButtonEnabled = CanOpenSelectionWindowExecute();
                }
            }
        };
        await _dialogService.ShowDialog(addNewType);
    }


    public void RemoveItem(int id)
    {
        var element = ItemsCollection.First(x => x.Id == id);
        ItemsCollection.Remove(element);
        if (ItemsCollection.Count == 0 && !IsButtonVisible)
        {
            BeingDisplayed = false;
        }
        if (IsButtonVisible)
        {
            IsButtonEnabled = CanOpenSelectionWindowExecute();

        }
        FilterProductsEvent?.Invoke(element.Id, element.Tekst);
    }

    public void AddItem(int id, string tekst)
    {
        var element = new LabelViewModel
        {
            Id = id,
            Tekst = tekst,
            RemoveCommand = new RelayCommand(() => { RemoveItem(id); })
        };

        int indexToInsert = FindIndexToInsert(element);
        ItemsCollection.Insert(indexToInsert, element);

        if (ItemsCollection.Count > 0)
        {
            BeingDisplayed = true;
        }
    }

    private int FindIndexToInsert(LabelViewModel item)
    {
        for (int i = 0; i < ItemsCollection.Count; i++)
        {
            if (ItemsCollection[i].Id > item.Id)
            {
                return i;
            }
        }
        return ItemsCollection.Count;
    }
}