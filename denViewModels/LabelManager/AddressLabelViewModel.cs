using denSharedLibrary;

using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Printers;
using SettingsKeptInFile;

namespace denViewModels;

public class AddressLabelViewModel : ObservableObject, ILabelTypeViewModel
{
    public ICommand DeleteCommand { get; set; }

    private int _width;

    public int Width
    {
        get => _width;
        set
        {
            if (!SetProperty(ref _width, value)) return;
            ImageWidth = Convert.ToInt32(Width / 25.4 * Dpi);

            if (Adres != null && Height == 0)
            {
                Image = addressLabel2Image.GenerateImages(Adres, Width, Height);
            }

            if (readyToSave)
            {
                _properties.Width = Width;
                LabelPropertiesManager.SaveLabelProperties(_properties);
            }
        }
    }

    public ICommand IncreaseWidth { get; set; }
    public ICommand DecreaseWidth { get; set; }

    private int _height;
    public int Height
    {
        get => _height;
        set
        {
            if (SetProperty(ref _height, value))
            {
                ImageHeight = Convert.ToInt32(Height / 25.4 * Dpi);
                if (Adres != null && Width == 0)
                {
                    Image = addressLabel2Image.GenerateImages(Adres, Width, Height);
                }

                if (readyToSave)
                {
                    _properties.Height = Height;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }
            }
        }
    }

    private byte[] _image;
    public byte[] Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }

    public AsyncRelayCommand PrintCommand { get; set; }

    private int _imagewidth;
    public int ImageWidth
    {
        get => _imagewidth;
        set => SetProperty(ref _imagewidth, value);
    }

    private int _imageHeight;
    public int ImageHeight
    {
        get => _imageHeight;
        set => SetProperty(ref _imageHeight, value);
    }

    private short _value;

    public short Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                OnPropertyChanged(nameof(ValueText));
            }
        }
    }

    public string ValueText
    {
        get => Value.ToString();
        set
        {
            if (short.TryParse(value, out short newValue) && newValue >= 0 && newValue <= 200)
            {
                Value = newValue;
            }
        }
    }

    private const float Dpi = 72;
    LabelProperties _properties;
    public ICommand IncreaseHeight { get; set; }
    public ICommand DecreaseHeight { get; set; }
    bool readyToSave = false;

    AddressLabel2Image addressLabel2Image;
    string Adres;
    string PrinterName;
    IDialogService _dialogService;

    private string _printLabelButtonText;
    public string PrintLabelButtonText
    {
        get => _printLabelButtonText;
        set => SetProperty(ref _printLabelButtonText, value);
    }
    private bool _showSlider;
    public bool ShowSlider
    {
        get => _showSlider;
        set => SetProperty(ref _showSlider, value);
    }

    public enum TypeOfAddressLabel
    {
        returnlabel,
        addresslabel
    }

    IPrintersService PrintersService;
    ISettingsService SettingsService;

    public AddressLabelViewModel(LabelProperties properties, string printerName, string adres, IDialogService dialogService,
        ISettingsService settingsService, IPrintersService printersService, TypeOfAddressLabel typeOfAddressLabel=TypeOfAddressLabel.addresslabel)
    {
        PrintersService = printersService;
        SettingsService = settingsService;
        _dialogService = dialogService;
        Height = properties.Height;
        Width = properties.Width;
        _properties = properties;
        Adres = adres;
        PrinterName = printerName;
        ImageWidth = Convert.ToInt32(Width / 25.4 * Dpi);
        ImageHeight = Convert.ToInt32(Height / 25.4 * Dpi);
        DecreaseWidth = new RelayCommand(() => { if (Width > 0) { Width--; } });
        IncreaseWidth = new RelayCommand(() => { if (Width < 160) { Width++; } });
        DecreaseHeight = new RelayCommand(() => { if (Height > 0) { Height--; } });
        IncreaseHeight = new RelayCommand(() => { if (Height < 100) { Height++; } });
        readyToSave = true;
        addressLabel2Image = new AddressLabel2Image();
        PrintCommand = new AsyncRelayCommand(PrintExecute);
        Image = addressLabel2Image.GenerateImages(Adres, Width, Height);
        if (properties.LabelType == LabelType.ReturnLabel)
        {
            PrintLabelButtonText = denLanguageResourses.Resources.Print;
            ShowSlider = true;
        }
        else
        {
            PrintLabelButtonText = denLanguageResourses.Resources.PrintExample;
            ShowSlider = false;
        }

        if (typeOfAddressLabel == TypeOfAddressLabel.returnlabel)
        {
            SettingsService.SettingsMessenger.Subscribe("businessname", AddressChanged);
        }
    }

    public void AddressChanged(string newaddress)
    {
        Adres = newaddress;
        Image = addressLabel2Image.GenerateImages(Adres, Width, Height);
    }

    public async Task PrintExecute()
    {
        if (ShowSlider)
        {
            var res = await _dialogService.ShowYesNoMessageBox(denLanguageResourses.Resources.ConfirmYourChoice, string.Format(denLanguageResourses.Resources.ConfirmPrintLabels, Value));
            if (res)
            {
                PrintersService.PrintAddressLabel(Width, Height, PrinterName, Value, Image);
            }
        }
        else
        {
            PrintersService.PrintAddressLabel(Width, Height, PrinterName, 1, Image);
        }
    }

    public void SetPrinterName(string printerName)
    {
        PrinterName=printerName;
    }
}