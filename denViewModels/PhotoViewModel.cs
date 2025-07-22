using denSharedLibrary;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.Models;


namespace denViewModels;

public class PhotoViewModel : ObservableObject
{
    public photo Photo { get; set; }
      
    public int PhotoId { get; set; }
    public int Position { get; set; }
    public string ImagePath { get; set; }
     
    public byte[] Image { get; set; }


    private string _picDesc;
    public string PicDesc
    {
        get => _picDesc;
        set => SetProperty(ref _picDesc, value);
    }

    private bool _canMoveUp;
    public bool CanMoveUp
    {
        get => PhotosEnabled&&_canMoveUp;
        set => SetProperty(ref _canMoveUp, value);
    }

    private bool _photosEnabled = true;
    public bool PhotosEnabled
    {
        get => _photosEnabled;
        set  
        {
            if (SetProperty(ref _photosEnabled, value))
            {
                OnPropertyChanged(nameof(CanMoveUp));
            }
        }
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    private bool _isLoaded;
    public bool IsLoaded
    {
        get => _isLoaded;
        set => SetProperty(ref _isLoaded, value);
    }

    private readonly IFileDialogService _fileDialogService;

    public PhotoViewModel(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
        SaveCommand = new AsyncRelayCommand(SavePhotoExecute);
    }

      

    public async Task SavePhotoExecute()
    {

        var stream = await _fileDialogService.SaveFileAsync("Save Image file", "JPEG Image|*.jpg");

        if (stream != null)
        {
            await using (stream.ConfigureAwait(false)) 
            {
                await stream.WriteAsync(Image, 0, Image.Length); 
            }
        }

            

    }

    public ICommand SaveCommand { get; set; }
    public ICommand DeleteCommand { get; set; }
    public ICommand MoveUpCommand { get; set; }


       
}