using denSharedLibrary;
using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkiaSharp;


namespace denViewModels;

public class PngAnimatorControlViewModel : ObservableObject
{
    private string _folder;
    private string _filePattern;
    private bool _going;

    public string Folder
    {
        get { return _folder; }
        set
        {
            if (SetProperty(ref _folder, value))
            {
                LoadFilesCommand.Execute(this);
            }

        }
    }

    public string FilePattern
    {
        get { return _filePattern; }
        set
        {
            if (SetProperty(ref _filePattern, value))
            {
                LoadFilesCommand.Execute(this);
            }
        }
    }

    public bool Going
    {
        get { return _going; }
        set
        {
            if (SetProperty(ref _going, value))
            {
                if (!filesRead)
                {
                    return;
                }
                if (value)
                {
                    _dispatcherTimer.Start(); 
                }
                else
                {
                    _dispatcherTimer.Stop(); 
                }
            }
        }
    }


    private SKBitmap _image;
    public SKBitmap Image
    {
        get => _image;
        set
        {
            SetProperty(ref _image, value);
        }
    }
    private void TimerTick(object sender, EventArgs e)
    {
        licznik++;
        var imageBytes = _cachedImages[$"{Folder}{FilePattern}"][licznik % 30];
        Image = SKBitmap.Decode(imageBytes);
    }

    public AsyncRelayCommand LoadFilesCommand { get; set; }
    private int licznik = 0;
     

    IDispatcherTimer _dispatcherTimer;
    public PngAnimatorControlViewModel(IDispatcherTimer dispatcherTimer)
    {
        _dispatcherTimer = dispatcherTimer;
        LoadFilesCommand = new AsyncRelayCommand(LoadFilesExecute);
        _dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000 / 30.0); // Ustawia interwał, aby wywoływać metodę 30 razy na sekundę
        _dispatcherTimer.Tick += TimerTick;
    }

    private Dictionary<int, byte[]> _images = new Dictionary<int, byte[]>();
    private static readonly Dictionary<string, Dictionary<int, byte[]>> _cachedImages = new Dictionary<string, Dictionary<int, byte[]>>();
    bool filesRead = false;


    public async Task LoadFilesExecute()
    {
        if (string.IsNullOrEmpty(_filePattern) || string.IsNullOrEmpty(Folder))
        {
            return;
        }
        string key = $"{Folder}{FilePattern}";

        if (_cachedImages.ContainsKey(key))
        {
            _images = _cachedImages[key];
            filesRead = true;
            return;
        }

        _images.Clear();
        for (int i = 1; ; i++)
        {
            var uj = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var filename = Folder + "\\" + FilePattern + i.ToString() + ".png";
            var filePath = uj + filename;

            if (!File.Exists(filePath))
            {
                break;
            }
            byte[] fileData = await File.ReadAllBytesAsync(filePath);
            _images[i - 1] = fileData;
        }

        _cachedImages[key] = _images;
        filesRead = true;
    }
}