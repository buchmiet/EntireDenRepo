using denModels;
using denSharedLibrary;
using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SettingsKeptInFile;

namespace denViewModels;

public class PrePrintLabelViewModel : ObservableObject, IAsyncDialogViewModel
{

    public event AsyncEventHandler RequestClose;


    private short _copies;
    public short Copies
    {
        get => _copies;
        set => SetProperty(ref _copies, value);
    }

    private KeyValuePair<string, short> _response;
    public KeyValuePair<string, short> Response
    {
        get => _response;
        set => SetProperty(ref _response, value);
    }

    public ObservableCollection<BoolString> LabelProps { get; set; } = new ObservableCollection<BoolString>();

    IDialogService _dialogService;

    public AsyncRelayCommand PerformTestCommand { get; set; }

    public async Task PerformTest()
    {
        if (LabelProps.Count == 0)
        {
            await _dialogService.ShowMessage(
                denLanguageResourses.Resources.NoLabelsDesignedMessage,
                denLanguageResourses.Resources.LabelManagerDesignRequiredMessage);

            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }



    public PrePrintLabelViewModel(IDialogService dialogService,ISettingsService settingsService)
    {

        _copies = 1;
        _dialogService = dialogService;
        var l = LabelPropertiesManager.GetLabelProperties().Where(p => p.LabelType == LabelType.ProductLabel).Select(p => p.LabelName);
        PerformTestCommand = new AsyncRelayCommand(PerformTest);


        foreach (var prop in l)
        {
            LabelProps.Add(new BoolString
            {
                Tick = false,
                Name = prop
            });
        }

        var defaultlabelResponse = settingsService.GetSetting("default_label");
        if (defaultlabelResponse.IsSuccess && LabelProps.Select(p => p.Name).Contains(defaultlabelResponse.GetValue<string>()))
        {
            LabelProps.First(p => p.Name.Equals(defaultlabelResponse.GetValue<string>())).Tick = true;
        }
        else
        {
            LabelProps.First().Tick = true;
        }
        PerformTestCommand.Execute(null);

    }

    public ICommand CancelCommand => new RelayCommand(CancelClick);
    public ICommand IncrementCopiesCommand => new RelayCommand(IncrementCopies);
    public ICommand DecrementCopiesCommand => new RelayCommand(DecrementCopies);
    public ICommand PrintBoxCommand => new RelayCommand(PrintBoxClick);
    public ICommand RadioButtonCommand => new RelayCommand<string>(LabelTypeClicked);

    public void LabelTypeClicked(string name)
    {
        foreach (var prop in LabelProps)
        {
            prop.Tick = prop.Name.Equals(name);
        }
    }

    private void CancelClick()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    private void IncrementCopies()
    {
        Copies++;
    }

    private void DecrementCopies()
    {
        if (Copies > 1)
            Copies--;
    }

    private void PrintBoxClick()
    {
        var odpo = LabelProps.First(p => p.Tick);
        Response = new KeyValuePair<string, short>(odpo.Name, Copies);
        RequestClose?.Invoke(this, EventArgs.Empty);
    }


}