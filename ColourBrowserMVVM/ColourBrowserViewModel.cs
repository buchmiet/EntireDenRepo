using ColourBrowserMVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColoursOperations;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.Models;
using static denSharedLibrary.Colours;
using DataServicesNET80.DatabaseAccessLayer;

namespace ColourBrowserMVVM;

public class ColourBrowserViewModel : INotifyPropertyChanged
{
    public IDatabaseAccessLayer _databaseAccessLayer;

    public int ChosenColour;
    public Dictionary<string, WriteableBitmap> originalSources = new();
    private ImageSource _finalEffect;
    private ImageSource _finalEffectTiny;
    private bool _isColorPicked;
    private bool _isColourFromListSelected = false;
    private bool _isDataLoaded = false;
    // saveTranslation
    private bool _isSaveTranslationEnabled;

    private double _malutkieHeight = 32;
    // malutkie
    private double _malutkieWidth = 32;

    private ObservableCollection<parametervalue> _nazwyKolorow;
    private System.Windows.Media.Color _pickedColor;
    private parametervalue _selectedColor;

    private string[] schematyKolorow = { "1.gif", "2.gif", "3.gif", "4.gif", "6.gif", "7.gif", "szwy.png", "stripes.gif", "flower.gif", "camo.gif", "mcrystal.gif", "leather.gif" };

    private colourtranslation Selectedcolourtranslation;
    private IColoursService _coloursService;

    public ColourBrowserViewModel(IDatabaseAccessLayer databaseAccessLayer,IColoursService coloursService)
    {
        _databaseAccessLayer = databaseAccessLayer;
        NazwyKolorow = new ObservableCollection<parametervalue>();
        AvailableColours = new();
        MakeSureItsReadyCommand = new AsyncRelayCommand(ReadyingExecute);
        SaveTranslationCommand = new AsyncRelayCommand(SaveTranslationExecute);
        _coloursService = coloursService;
    }

    public async Task SaveTranslationExecute()
    {
        var ga = await _databaseAccessLayer. AddOrUpdateColourTranslation(Selectedcolourtranslation);
        _coloursService.RaiseColourEvent(ga.kodKoloru);

        Selectedcolourtranslation = new();
        FinalEffect = null;
        AvailableColours.Clear();
        IsColorPicked = false;
        SelectedColor = null;
        IsColourFromListSelected= false;
        IsSaveTranslationEnabled = false;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<AvailableColourViewModel> AvailableColours { get; set; }

    public ObservableCollection<ColourSchemeViewModel> ColourSchemes { get; set; } = new();

    public ImageSource FinalEffect
    {
        get => _finalEffect;
        set
        {
            _finalEffect = value;
            FinalEffectTiny = value;
            NotifyPropertyChanged(nameof(FinalEffect));
        }
    }

    public ImageSource FinalEffectTiny
    {
        get => _finalEffectTiny;
        set
        {
            _finalEffectTiny = value;
            NotifyPropertyChanged(nameof(FinalEffectTiny));
        }
    }

    public bool IsColorPicked
    {
        get => _isColorPicked;
        set
        {
            _isColorPicked = value;
            NotifyPropertyChanged(nameof(IsColorPicked));
        }
    }

    public bool IsColourFromListSelected
    {
        get => _isColourFromListSelected;
        set
        {
            _isColourFromListSelected = value;
            NotifyPropertyChanged(nameof(IsColourFromListSelected));
        }
    }

    public bool IsDataLoaded
    {
        get => _isDataLoaded;
        set
        {
            _isDataLoaded = value;
            NotifyPropertyChanged(nameof(IsDataLoaded));
        }
    }

    public bool IsSaveTranslationEnabled
    {
        get => _isSaveTranslationEnabled;
        set
        {
            _isSaveTranslationEnabled = value;
            NotifyPropertyChanged(nameof(IsSaveTranslationEnabled));
        }
    }

    public ICommand MakeSureItsReadyCommand { get; set; }
    public ICommand SaveTranslationCommand { get; set; }
        

    public double MalutkieHeight
    {
        get => _malutkieHeight;
        set
        {
            _malutkieHeight = value;
            NotifyPropertyChanged(nameof(MalutkieHeight));
        }
    }

    public double MalutkieWidth
    {
        get => _malutkieWidth;
        set
        {
            _malutkieWidth = value;
            NotifyPropertyChanged(nameof(MalutkieWidth));
        }
    }

    public ObservableCollection<parametervalue> NazwyKolorow
    {
        get => _nazwyKolorow;
        set
        {
            _nazwyKolorow = value;
            NotifyPropertyChanged(nameof(NazwyKolorow));
        }
    }

    public System.Windows.Media.Color PickedColour
    {
        get => _pickedColor;
        set
        {
            _pickedColor = value;
            NotifyPropertyChanged(nameof(PickedColour));

            byte[] bytearray = new byte[4];
            bytearray[0] = PickedColour.B;
            bytearray[1] = PickedColour.G;
            bytearray[2] = PickedColour.R;
            bytearray[3] = 255;
            if (ChosenColour == 1)
            {
                Selectedcolourtranslation.col1 = BitConverter.ToInt32(bytearray, 0);
            }
            if (ChosenColour == 2)
            {
                Selectedcolourtranslation.col2 = BitConverter.ToInt32(bytearray, 0);
            }
            if (ChosenColour == 3)
            {
                Selectedcolourtranslation.col3 = BitConverter.ToInt32(bytearray, 0);
            }
            if (ChosenColour == 4)
            {
                Selectedcolourtranslation.col4 = BitConverter.ToInt32(bytearray, 0);
            }
            var beempe = RenderTranslation(Selectedcolourtranslation);
            FinalEffect = beempe;
            IsSaveTranslationEnabled = true;
            ShowColours(new WriteableBitmap(beempe));
        }
    }

    public parametervalue SelectedColor
    {
        get { return _selectedColor; }
        set
        {
            _selectedColor = value;
            NotifyPropertyChanged(nameof(SelectedColor));
            ColourSelected();
        }
    }
    public static List<byte[]> FindColours(byte[] bsource)
    {
        var zwrotka = new List<byte[]>();
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                var buf = new byte[4];

                Buffer.BlockCopy(bsource, ((y * 128) + x) * 4, buf, 0, 4);

                bool jest = false;
                foreach (var bufek in zwrotka)
                {
                    if (bufek[0] == buf[0] &&
                        bufek[1] == buf[1] &&
                        bufek[2] == buf[2] &&
                        bufek[3] == buf[3]
                       )
                    {
                        jest = true;
                    }
                }

                if (!jest)
                {
                    zwrotka.Add(buf);
                }
            }
        }
        return zwrotka.ToList();
    }

    public static byte[] ReplaceColours(byte[] obrazek, Dictionary<int, int> translacje)
    {
        var zwrotka = new byte[obrazek.Length];
        for (int y = 0; y < 128; y++)
        {
            for (int x = 0; x < 128; x++)
            {
                var buf = new byte[4];
                Buffer.BlockCopy(obrazek, ((y * 128) + x) * 4, buf, 0, 4);
                int result1 = BitConverter.ToInt32(buf, 0);
                if (translacje.ContainsKey(result1))
                {
                    byte[] nowykol = BitConverter.GetBytes(translacje[result1]);
                    var nowykol2 = new byte[4];
                    nowykol2[0] = nowykol[3];
                    nowykol2[1] = nowykol[2];
                    nowykol2[2] = nowykol[1];
                    nowykol2[3] = nowykol[0];
                    Buffer.BlockCopy(nowykol, 0, zwrotka, ((y * 128) + x) * 4, 4);
                }
                else
                {
                    Buffer.BlockCopy(buf, 0, zwrotka, ((y * 128) + x) * 4, 4);
                }
            }
        }
        return zwrotka;
    }

    //gdy uzytkownik klika na kolor z dostepnych kolorow
    public void ColourClicked(RGB rgb, int i)
    {
        IsColorPicked = true;
        ChosenColour = i;
        PickedColour = System.Windows.Media.Color.FromArgb(255, rgb.R, rgb.G, rgb.B);
    }

    public void ColourSchemeClicked(string sk)
    {
        ChosenColour = 0;
        IsColorPicked = false;
        IsSaveTranslationEnabled = true;
        var ux = ShowColours(originalSources[sk]).ToArray();
        Selectedcolourtranslation = new colourtranslation
        {
            Id=Selectedcolourtranslation.Id,
            kodKoloru=SelectedColor.parameterValueID,
            schemat = sk,
            col1 = BitConverter.ToInt32(ux[0], 0)
        };
        if (ux.Length > 1)
        {
            Selectedcolourtranslation.col2 = BitConverter.ToInt32(ux[1], 0);
        }
        if (ux.Length > 2)
        {
            Selectedcolourtranslation.col3 = BitConverter.ToInt32(ux[2], 0);
        }
        if (ux.Length == 4)
        {
            Selectedcolourtranslation.col3 = BitConverter.ToInt32(ux[3], 0);
        }
        FinalEffect = ColourSchemes.First(p => p.Name.Equals(sk)).Image;
    }

    //gdy uzytkownik klika na kolor z listy
    public async void ColourSelected()
    {
        if (SelectedColor == null)
        {
            return;
        }
        IsColourFromListSelected = true;
        ChosenColour = 0;
        IsColorPicked = false;
        IsSaveTranslationEnabled = false;
         
        if ((await _databaseAccessLayer.ColourTranslations()).ContainsKey(SelectedColor.parameterValueID))
        {
            var translation =(await _databaseAccessLayer.ColourTranslations())[SelectedColor.parameterValueID];
            //   translation.Id = Selectedcolourtranslation.Id;
            //   translation.kodKoloru = Selectedcolourtranslation.kodKoloru;
            Selectedcolourtranslation = translation;

            var beempe = RenderTranslation(translation);
            FinalEffect = beempe;
            ShowColours(new WriteableBitmap(beempe));
        }
        else
        {
            Selectedcolourtranslation = new();
            FinalEffect = null;
            AvailableColours.Clear();
            IsColorPicked = false;
        }
    }
    public async Task ReadyingExecute()
    {
        if (IsDataLoaded) { return; }

        await _databaseAccessLayer.GetPackage(1);
        NazwyKolorow = new ObservableCollection<parametervalue>( (await  _databaseAccessLayer.cechyValues().ConfigureAwait(false))[_databaseAccessLayer.colourProperty]);
        int i = 0;
        foreach (var sk in schematyKolorow)
        {
            originalSources.Add(sk, File2WriteableBitmap(sk));
            var os = new ColourSchemeViewModel
            {
                Image = File2Bitmap(sk),
                SchemeSelectedCommand = new RelayCommand(() => ColourSchemeClicked(sk)),
                Name = sk
            };
            i++;
            ColourSchemes.Add(os);
        }
        IsDataLoaded = true;
    }
    public BitmapSource RenderTranslation(colourtranslation kolek)
    {
        var bufor = new byte[128 * 128 * 4];
        WriteableBitmap bmp = originalSources[kolek.schemat];
        bmp.CopyPixels(bufor, 512, 0);
        byte[][] starekolory = FindColours(bufor).ToArray();
        var tempik = new Dictionary<int, int>
        {
            { BitConverter.ToInt32(starekolory[0], 0), kolek.col1 }
        };
        if (kolek.col2 != null)
        {
            tempik.Add(BitConverter.ToInt32(starekolory[1], 0), (int)kolek.col2);
        }
        if (kolek.col3 != null)
        {
            tempik.Add(BitConverter.ToInt32(starekolory[2], 0), (int)kolek.col3);
        }
        if (kolek.col4 != null)
        {
            tempik.Add(BitConverter.ToInt32(starekolory[3], 0), (int)kolek.col3);
        }
        bufor = ReplaceColours(bufor, tempik);
        BitmapSource bitmap = BitmapSource.Create(128, 128, 96, 96, PixelFormats.Bgra32, null, bufor, 512);
        return bitmap;
    }

    public List<byte[]> ShowColours(WriteableBitmap bitmapa)
    {
        AvailableColours.Clear();
        byte[] bitma = new byte[128 * 128 * 4];
        bitmapa.CopyPixels(bitma, 512, 0);

        var kolorki = FindColours(bitma);
        int i = 1;
        foreach (var kol in kolorki)
        {
            var rgb = new RGB
            {
                R = kol[2],
                G = kol[1],
                B = kol[0]
            };
            int currentI = i;
            var sp = new AvailableColourViewModel
            {
                Color = rgb,
                OldColor = rgb,
                ColourId = i,
                ColourSelectedCommand = new RelayCommand(() => ColourClicked(rgb, currentI)),
            };

            AvailableColours.Add(sp);
            i++;
        }
        return kolorki;
    }

    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static BitmapImage File2Bitmap(string nazwa)
    {
        string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\" + nazwa);

        BitmapImage image = new BitmapImage();
        using (FileStream fs = File.OpenRead(path))
        {
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = fs;
            image.EndInit();
        }

        return image;
    }

    private static WriteableBitmap File2WriteableBitmap(string nazwa)
    {
        string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\" + nazwa);
        BitmapSource srcImage;
        using (FileStream fs = File.OpenRead(path))
        {
            srcImage = BitmapDecoder.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames[0];
        }
        if (srcImage.Format != PixelFormats.Bgra32)
        {
            srcImage = new FormatConvertedBitmap(srcImage, PixelFormats.Bgra32, null, 0);
        }

        return new WriteableBitmap(srcImage);
    }
}