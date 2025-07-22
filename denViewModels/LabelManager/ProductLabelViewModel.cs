using denSharedLibrary;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Printers;
using static denSharedLibrary.Colours;

namespace denViewModels;

public class ProductLabelViewModel : ObservableObject, ILabelTypeViewModel
{
    private string _labelType;
    public string LabelType { get => _labelType;
        set => SetProperty(ref _labelType, value);
    }

    private int _width;

    public int Width
    {
        get => _width;
        set
        {
            if (SetProperty(ref _width, value))
            {
                ImageWidth = Convert.ToInt32(Width / 25.4 * Dpi);

                Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);
                if (readyToSave)
                {
                    _properties.Width = Width;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }
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
                Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);
                if (readyToSave)
                {
                    _properties.Height = Height;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }
            }
        }
    }

    public ICommand IncreaseHeight { get; set; }
    public ICommand DecreaseHeight { get; set; }

    private int _topFont;
    public int TopFont
    {
        get => _topFont;
        set
        {
            if (SetProperty(ref _topFont, value))
            {
                Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);
                if (readyToSave)
                {
                    _properties.TopFont = TopFont;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }
            }
        }
    }

    public ICommand IncreaseTopFont { get; set; }
    public ICommand DecreaseTopFont { get; set; }

    private float _bottomFont;
    public float BottomFont
    {
        get => _bottomFont;
        set
        {
            if (SetProperty(ref _bottomFont, value))
            {
                Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);
                if (readyToSave)
                {
                    _properties.BottomFont = BottomFont;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }
            }
        }
    }

    public ICommand IncreaseBottomFont { get; set; }
    public ICommand DecreaseBottomFont { get; set; }

    private float _largeFont;
    public float LargeFont
    {
        get => _largeFont;
        set
        {
            if (SetProperty(ref _largeFont, value))
            {
                Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);
                if (readyToSave)
                {
                    _properties.LargeFont = LargeFont;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }
            }
        }
    }

    public ICommand IncreaseLargeFont { get; set; }
    public ICommand DecreaseLargeFont { get; set; }

    private float _lesserFont;
    public float LesserFont
    {
        get => _lesserFont;
        set
        {
            if (SetProperty(ref _lesserFont, value))
            {
                Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);
                if (readyToSave)
                {
                    _properties.LesserFont = LesserFont;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }
            }
        }
    }

    public ICommand IncreaseLesserFont { get; set; }
    public ICommand DecreaseLesserFont { get; set; }

    private byte[] _image;
    public byte[] Image
    {
        get { return _image; }
        set { SetProperty(ref _image, value); }
    }

    private string _topSmallText;
    public string TopSmallText
    {
        get { return _topSmallText; }
        set { SetProperty(ref _topSmallText, value); }
    }
    private string _centralLargeText;
    public string CentralLargeText
    {
        get { return _centralLargeText; }
        set { SetProperty(ref _centralLargeText, value); }
    }

    private string _centralSmallText;
    public string CentralSmallText
    {
        get { return _centralSmallText; }
        set { SetProperty(ref _centralSmallText, value); }
    }

    private string _subtitle;
    public string Subtitle
    {
        get { return _subtitle; }
        set { SetProperty(ref _subtitle, value); }
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

    private bool _landscape;
    public bool Landscape
    {
        get => _landscape;
        set
        {
            if (SetProperty(ref _landscape, value))
            {
                _properties.Landscape = Landscape;
                LabelPropertiesManager.SaveLabelProperties(_properties);
            }
        }
    }

    private int _centralLineSpacing;
    public int CentralLineSpacing
    {
        get => _centralLineSpacing;
        set
        {
            if (SetProperty(ref _centralLineSpacing, value))
            {
                Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);
                if (readyToSave)
                {
                    _properties.CentralLineSpacing = CentralLineSpacing;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }
            }
        }
    }

    public ICommand IncreaseCentralLineSpacing { get; set; }
    public ICommand DecreaseCentralLineSpacing { get; set; }

    private int _bottomMargin;
    public int BottomMargin
    {
        get => _bottomMargin;
        set
        {
            if (SetProperty(ref _bottomMargin, value))
            {
                Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);
                if (readyToSave)
                {
                    _properties.BottomMargin = BottomMargin;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }
            }
        }
    }
    public ICommand IncreaseBottomMargin { get; set; }
    public ICommand DecreaseBottomMargin { get; set; }

    private int _topMargin;
    public int TopMargin
    {
        get => _topMargin;
        set
        {
            if (SetProperty(ref _topMargin, value))
            {
                Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);
                if (readyToSave)
                {
                    _properties.TopMargin = TopMargin;
                    LabelPropertiesManager.SaveLabelProperties(_properties);
                }

            }
        }
    }

    public ICommand IncreaseTopMargin { get; set; }
    public ICommand DecreaseTopMargin { get; set; }

    private const float Dpi = 72;


    LabelProperties _properties;
    //   public LabelTypeViewModel(IDialogService dialogService, IBoxLabelToImageByteArray boxLabelToImageByteArray,string topSmallText, string centralLargeText, string centralSmallText, string subtitle, LabelProperties properties)

    private string PrinterName;
    private LabelNamePack _labelNamePack;
    bool readyToSave = false;
    public ICommand DeleteCommand { get; set; }
    IPrintersService PrintersService;


    public ProductLabelViewModel(LabelNamePack labelNamePack, LabelProperties properties, string printerName,IPrintersService printersService)
    {
        PrintersService= printersService;
        _labelNamePack = labelNamePack;
        TopSmallText = _labelNamePack.Toptext;
        CentralLargeText = _labelNamePack.CentralLargeText;
        CentralSmallText = _labelNamePack.CentralSmallText;
        Subtitle = _labelNamePack.Bottomtext;
        LabelType = properties.LabelName;
        TopFont = (int)properties.TopFont;
        BottomFont = (int)properties.BottomFont;
        LargeFont = (int)properties.LargeFont;
        LesserFont = (int)properties.LesserFont;
        Height = properties.Height;
        Width = properties.Width;
        _properties = properties;
        PrinterName = printerName;
        CentralLineSpacing = (int)properties.CentralLineSpacing;
        BottomMargin = (int)properties.BottomMargin;
        TopMargin = (int)properties.TopMargin;
        ImageWidth = Convert.ToInt32(Width / 25.4 * Dpi);
        ImageHeight = Convert.ToInt32(Height / 25.4 * Dpi);
        Landscape = properties.Landscape;
        PrintCommand = new AsyncRelayCommand(PrintExecute);
        DecreaseTopFont = new RelayCommand(() => { if (TopFont > 0) { TopFont--; } });
        IncreaseTopFont = new RelayCommand(() => { if (TopFont < 128) { TopFont++; } });
        DecreaseLesserFont = new RelayCommand(() => { if (LesserFont > 0) { LesserFont--; } });
        IncreaseLesserFont = new RelayCommand(() => { if (LesserFont < 128) { LesserFont++; } });
        DecreaseLargeFont = new RelayCommand(() => { if (LargeFont > 0) { LargeFont--; } });
        IncreaseLargeFont = new RelayCommand(() => { if (LargeFont < 128) { LargeFont++; } });
        DecreaseBottomFont = new RelayCommand(() => { if (BottomFont > 0) { BottomFont--; } });
        IncreaseBottomFont = new RelayCommand(() => { if (BottomFont < 128) { BottomFont++; } });
        DecreaseHeight = new RelayCommand(() => { if (Height > 0) { Height--; } });
        IncreaseHeight = new RelayCommand(() => { if (Height < 100) { Height++; } });
        DecreaseWidth = new RelayCommand(() => { if (Width > 0) { Width--; } });
        IncreaseWidth = new RelayCommand(() => { if (Width < 160) { Width++; } });
        DecreaseTopMargin = new RelayCommand(() => { if (TopMargin > 0) { TopMargin--; } });
        IncreaseTopMargin = new RelayCommand(() => { TopMargin++; });
        DecreaseBottomMargin = new RelayCommand(() => { if (BottomMargin > 0) { BottomMargin--; } });
        IncreaseBottomMargin = new RelayCommand(() => { BottomMargin++; });
        DecreaseCentralLineSpacing = new RelayCommand(() => { if (CentralLineSpacing > 0) { CentralLineSpacing--; } });
        IncreaseCentralLineSpacing = new RelayCommand(() => { CentralLineSpacing++; });
        readyToSave = true;
        Image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth, ImageHeight, _labelNamePack, true, 1);

    }



    private RGB _topTextColour = new RGB { R = 255, G = 0, B = 0 };
    public RGB TopTextColour
    {
        get => _topTextColour;
        set => SetProperty(ref _topTextColour, value);
    }

    private RGB _bottomTextColour = new RGB { R = 0, G = 0, B = 0 };
    public RGB BottomTextColour
    {
        get => _bottomTextColour;
        set => SetProperty(ref _bottomTextColour, value);
    }

    private RGB _centralLargeTextColour = new RGB { R = 0, G = 255, B = 0 };
    public RGB CentralLargeTextColour
    {
        get => _centralLargeTextColour;
        set => SetProperty(ref _centralLargeTextColour, value);
    }

    private RGB _centralSmallTextColour = new RGB { R = 0, G = 0, B = 255 };
    public RGB CentralSmallTextColour
    {
        get => _centralSmallTextColour;
        set => SetProperty(ref _centralSmallTextColour, value);
    }

    public async Task PrintExecute()
    {
        var _image = LabelPropertiesManager.PlaceStringsOnCanvas(BottomTextColour, TopTextColour, CentralLargeTextColour, CentralSmallTextColour, _properties, ImageWidth * 4, ImageHeight * 4, _labelNamePack, false, 4);
        PrintersService.PrintLabel(_properties, PrinterName, 1, _image);
    }

    public void SetPrinterName(string printerName)
    {
        PrinterName=printerName;
    }
}