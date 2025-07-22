using denLanguageResourses;
using denMethods;
using denModels;
using denSharedLibrary;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SettingsKeptInFile;
using Printers;
using DataServicesNET80.DatabaseAccessLayer;
using SkiaSharp;

namespace denViewModels;

public class Cn22SettingsViewModel : ObservableObject
{
    private string cn22LabelPrinter;

    private string nameOfTheOnlyOneProduct;
    private string priceForTheOnlyOneProduct;
    private string weightForTheOnlyOneProduct;
    
    private string selectedFileWithSignature;

    public string CN22LabelPrinter
    {
        get => cn22LabelPrinter;
        set => SetProperty(ref cn22LabelPrinter, value);
    }


    private bool _printLinesFirstUse = true;
    private PrintLines _printLines;
    public PrintLines PrintLines
    {
        get => _printLines;
        set
        {
            if (SetProperty(ref _printLines, value) && value != null)
            {

                if (!_printLinesFirstUse)
                {
                    DebounceSave();
                }
                else
                {
                    _printLinesFirstUse = false;
                }
            }
        }
    }


    private bool nameOfTheOnlyOneProductFirstUse = true;
    public string NameOfTheOnlyOneProduct
    {
        get => nameOfTheOnlyOneProduct;
        set
        {
            if (SetProperty(ref nameOfTheOnlyOneProduct, value) && value != null)
            {

                if (!nameOfTheOnlyOneProductFirstUse)
                {
                    DebounceSave();
                }
                else
                {
                    nameOfTheOnlyOneProductFirstUse = false;
                }
            }
        }
    }

    private bool priceForTheOnlyOneProductFirstUse = true;
    public string PriceForTheOnlyOneProduct
    {
        get => priceForTheOnlyOneProduct;
        set
        {
            if (SetProperty(ref priceForTheOnlyOneProduct, value) && value != null)
            {

                if (!priceForTheOnlyOneProductFirstUse)
                {
                    DebounceSave();
                }
                else
                {
                    priceForTheOnlyOneProductFirstUse = false;
                }
            }
        }
    }

    private bool _weightForTheOnlyOneProductFirstUse = true;
    public string WeightForTheOnlyOneProduct
    {
        get => weightForTheOnlyOneProduct;
        set
        {
            if (!SetProperty(ref weightForTheOnlyOneProduct, value) || value == null) return;
            if (!_weightForTheOnlyOneProductFirstUse)
            {
                DebounceSave();
            }
            else
            {
                _weightForTheOnlyOneProductFirstUse = false;
            }
        }
    }

    private bool _displayedCurrencyFirstUse = true;

    private DisplayedCurrency _displayedCurrency;
    public DisplayedCurrency DisplayedCurrency
    {
        get => _displayedCurrency;
        set
        {
            if (!SetProperty(ref _displayedCurrency, value) || value == null) return;
            if (!_displayedCurrencyFirstUse)
            {
                DebounceSave();
            }
            else
            {
                _displayedCurrencyFirstUse = false;
            }
        }
    }

    private bool _useSignatureFileFirstUse = true;
    private bool _useSignatureFile;
    public bool UseSignatureFile
    {
        get => _useSignatureFile;
        set
        {
            if (!SetProperty(ref _useSignatureFile, value) || !value) return;
            if (!_useSignatureFileFirstUse)
            {
                DebounceSave();
            }
            else
            {
                _useSignatureFileFirstUse = false;
            }
        }
    }

    private bool selectedFileWithSignatureFirstUse = true;
    public string SelectedFileWithSignature
    {
        get => selectedFileWithSignature;
        set
        {
            if (!SetProperty(ref selectedFileWithSignature, value) || value == null) return;
            if (!selectedFileWithSignatureFirstUse)
            {
                DebounceSave();
            }
            else
            {
                selectedFileWithSignatureFirstUse = false;
            }
        }
    }

    private bool _nameOfTheSenderFirstUse = true;
    private string _nameOfTheSender;
    public string NameOfTheSender
    {
        get => _nameOfTheSender;
        set
        {
            if (!SetProperty(ref _nameOfTheSender, value) || value == null) return;
            if (!_nameOfTheSenderFirstUse)
            {
                DebounceSave();
            }
            else
            {
                _nameOfTheSenderFirstUse = false;
            }
        }
    }

    private bool _selectedContentTypeFirstUse = true;
    private string _selectedContentType;
    public string SelectedContentType
    {
        get => _selectedContentType;
        set
        {
            if (!SetProperty(ref _selectedContentType, value) || value == null) return;
            if (!_selectedContentTypeFirstUse)
            {
                DebounceSave();
            }
            else
            {
                _selectedContentTypeFirstUse = false;
            }
        }
    }


    private readonly Timer _debounceTimer;
    private void DebounceSave()
    {
        _debounceTimer.Change(500, Timeout.Infinite); // Opóźnienie 2 sekundy przed zapisem
    }

    private void DebounceTimerCallback(object state)
    {
        SaveSettings();
    }

    private void SaveSettings()
    {
        Dictionary<string, string> settingsToUpdate = new Dictionary<string, string>();

        if (PrintLines == PrintLines.ForAllSoldProducts)
        {
            settingsToUpdate.Add("cn22productsshown", "allproducts");
        }
        else
        {
            settingsToUpdate.Add("cn22productsshown", "oneproduct");
        }

        if (NameOfTheSender != null)
        {
            settingsToUpdate.Add("cn22nameofthesender", NameOfTheSender);
        }

        if (NameOfTheOnlyOneProduct != null)
        {
            settingsToUpdate.Add("cn22nameofoneproduct", NameOfTheOnlyOneProduct);
        }

        if (PriceForTheOnlyOneProduct != null)
        {
            settingsToUpdate.Add("cn22priceofoneproduct", PriceForTheOnlyOneProduct);
        }

        if (WeightForTheOnlyOneProduct != null)
        {
            settingsToUpdate.Add("cn22weightofoneproduct", WeightForTheOnlyOneProduct);
        }

        if (DisplayedCurrency == DisplayedCurrency.OrderCurrency)
        {
            settingsToUpdate.Add("cn22currency", "order");
        }
        else
        {
            settingsToUpdate.Add("cn22currency", "app");
        }

        if (SelectedContentType != null)
        {
            settingsToUpdate.Add("cn22contenttype", SelectedContentType);
        }

        if (UseSignatureFile != null)
        {
            settingsToUpdate.Add("cn22usesignaturefile", UseSignatureFile ? "true" : "false");

            if (UseSignatureFile && SelectedFileWithSignature != null)
            {
                settingsToUpdate.Add("cn22signaturefile", SelectedFileWithSignature);
            }
        }
        SettingsService.UpdateSettings(settingsToUpdate);
        ModifyImage();
    }


    private ObservableCollection<string> _largePrinters;

    public ObservableCollection<string> LargePrinters
    {
        get => _largePrinters;
        set => SetProperty(ref _largePrinters, value);
    }
    private string _selectedLargePrinter;
    public ICommand Show4x6SettingsCommand { get; }
    private byte[] _image;
    public byte[] Image
    {
        get => _image;
        set => SetProperty(ref _image, value);
    }



    public ObservableCollection<string> Cn22Content { get; set; } = [];






    private int _imageWidth;
    public int ImageWidth
    {
        get => _imageWidth;
        set => SetProperty(ref _imageWidth, value);
    }
    private int _imageHeight;
    public int ImageHeight
    {
        get => _imageHeight;
        set => SetProperty(ref _imageHeight, value);
    }


    public ICommand LoadDataCommand { get; }
    public ICommand LoadSignatureFileCommand { get; }
    public ICommand PrepareFieldsCommand { get; }
    public static IFileDialogService _fileDialogService;

    public ICommand PrintCN22Command { get; set; }

    public void PrintCN22Execute()
    {
        PrintersService.Print4x6Label(SelectedLargePrinter, 1, Image);
    }

    private ISettingsService SettingsService;
    private IPrintersService PrintersService;
    public IDialogService _dialogService;
    public static IDatabaseAccessLayer _databaseAccessLayer;

    public Cn22SettingsViewModel(ISettingsService settingsService,IPrintersService printersService,IDialogService dialogService,IDatabaseAccessLayer databaseAccessLayer)
    {
        PrintersService = printersService;
        SettingsService = settingsService;
        _dialogService = dialogService;
        _databaseAccessLayer = databaseAccessLayer;
        ImageWidth = ObliczDocelowaWysokoscWPikselach(GetDpi()[0], 100);
        ImageHeight = ObliczDocelowaWysokoscWPikselach(GetDpi()[0], 160);
        _debounceTimer = new Timer(DebounceTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        LoadDataCommand = new AsyncRelayCommand(LoadDataExecute);
        PrintCN22Command = new RelayCommand(PrintCN22Execute);
        Show4x6SettingsCommand = new AsyncRelayCommand(Show4x6SettingsExecute);
        var LPrinters = PrintersService.GetPrinters("4x6Labelpriner");
        LargePrinters = new ObservableCollection<string>(LPrinters.Value);
        LoadSignatureFileCommand = new AsyncRelayCommand(GetFileWithSignature);
        if (LPrinters.Key != null && LPrinters.Value.Contains(LPrinters.Key))
        {
            SelectedLargePrinter = LPrinters.Key;
        }
        if (LargePrinters.Count > 0)
        {

            if (LPrinters.Key != null && LPrinters.Value.Contains(LPrinters.Key))
            {
                LargePrintersMenuEnabled = true;
                EnableLargePrinterMenuButton = Resources.Disable4x6LabelPrinter;
            }
            else
            {
                EnableLargePrinterMenuButton = Resources.Enable4x6LabelPrinter;
                LargePrintersMenuEnabled = false;
            }
        }
        PrepareFieldsCommand = new AsyncRelayCommand(PrepareFields);
        PrepareFieldsCommand.Execute(null);
        ModifyImage();
    }

    CN22PropertiesPack Cn22Pack;
    public async Task PrepareFields()
    {
        Cn22Pack = PrintersService.PrepareCN22Fields();
        Cn22Content =
        [
            "Gift",
            "Documents",
            "Sale of Goods",
            "Commercial Sample",
            "Returned Goods"
        ];
        SelectedContentType = Cn22Pack.SelectedContentType;
        UseSignatureFile = Cn22Pack.UseSignatureFile;
        if (UseSignatureFile && Cn22Pack.SignatureFileBitmap == null)
        {
            await _dialogService.ShowMessage(Resources.ErrorTitle, Resources.InvalidImageFormat);
            UseSignatureFile = false;
            SelectedFileWithSignature = "";
        }
        else
        {
            SignatureFileBitmap = Cn22Pack.SignatureFileBitmap;
            SelectedFileWithSignature = Cn22Pack.SelectedFileWithSignature;
        }
        PrintLines = Cn22Pack.PrintLines;
        NameOfTheSender = Cn22Pack.NameOfTheSender;
        NameOfTheOnlyOneProduct = Cn22Pack.NameOfTheOnlyOneProduct;
        PriceForTheOnlyOneProduct = Cn22Pack.PriceForTheOnlyOneProduct;
        WeightForTheOnlyOneProduct = Cn22Pack.WeightForTheOnlyOneProduct;
        DisplayedCurrency = Cn22Pack.DisplayedCurrency;
        SelectedContentType = Cn22Pack.SelectedContentType;
        UseSignatureFile = Cn22Pack.UseSignatureFile;

    }

      

    public static float[] GetDpi()
    {
        using (Graphics graphics = Graphics.FromHwnd(nint.Zero))
        {
            float dpiX = graphics.DpiX;
            float dpiY = graphics.DpiY;
            float[] result = [dpiX = dpiX, dpiY = dpiY];
            return result;
        }
    }

    static int ObliczDocelowaWysokoscWPikselach(float dpi, float wysokoscWMilimetrach)
    {
        float wysokoscWCale = wysokoscWMilimetrach / 25.4f;
        int docelowaWysokoscWPikselach = (int)(wysokoscWCale * dpi);

        return docelowaWysokoscWPikselach;
    }

    public void ModifyImage()
    {
        if (NameOfTheSender == null) return;
        var komplet = MockOrderGenerator.GenerateRandom();
        Cn22Pack = PrintersService.PrepareCN22Fields();
        Image = PrintersService.CreateCN22Image(Cn22Pack, komplet);
          
    }

    public async Task GetFileWithSignature()
    {
        var fileName = await _fileDialogService.OpenFileAsync(Resources.LoadFileWithSignature, "Png Image|*.png|Bitmap Image|*.bmp|JPEG Image|*.jpg");
        if (fileName != null)
        {
            try
            {
                await using (var stream = new FileStream(fileName, FileMode.Open))
                {
                    SignatureFileBitmap = SKBitmap.Decode(stream);
                }
                SelectedFileWithSignature = fileName;
            }
            catch (Exception)
            {
                await _dialogService.ShowMessage(Resources.ErrorTitle, Resources.InvalidImageFormat);
            }
        }
    }

    SKBitmap SignatureFileBitmap { get; set; }


        

    private bool _isReady = false;
    public bool IsReady
    {
        get => _isReady;
        set => SetProperty(ref _isReady, value);
    }

    public async Task LoadDataExecute()
    {
        await _databaseAccessLayer.GetPackage(1);
        IsReady = true;
    }

    private string _enableLargePrinterMenuButton;
    public string EnableLargePrinterMenuButton
    {
        get => _enableLargePrinterMenuButton;
        set => SetProperty(ref _enableLargePrinterMenuButton, value);
    }

    private bool _largePrintersMenuEnabled = false;
    public bool LargePrintersMenuEnabled
    {
        get => _largePrintersMenuEnabled;
        set => SetProperty(ref _largePrintersMenuEnabled, value);
    }

    public async Task Show4x6SettingsExecute()
    {

        if (LargePrinters.Count() == 0)
        {
            await _dialogService.ShowMessage(Resources.NoPrintersInstalled, Resources.NoPrintersInstalledMessage);
            return;
        }
        if (LargePrintersMenuEnabled)
        {
            LargePrintersMenuEnabled = false;
            EnableLargePrinterMenuButton = Resources.Enable4x6LabelPrinter;
            SettingsService.UpdateSetting("4x6Labelpriner", null);

        }
        else
        {
            LargePrintersMenuEnabled = true;
            EnableLargePrinterMenuButton = Resources.Disable4x6LabelPrinter;
        }


    }

    public string SelectedLargePrinter
    {
        get => _selectedLargePrinter;
        set
        {
            if (SetProperty(ref _selectedLargePrinter, value))
            {
                if (value != null)
                {
                    SettingsService.UpdateSetting("4x6Labelpriner", SelectedLargePrinter);
                }

            }
        }
    }
}