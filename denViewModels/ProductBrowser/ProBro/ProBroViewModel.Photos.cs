using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using DataServicesNET80.Models;
using denMethods;
using denModels;

namespace denViewModels.ProductBrowser.ProBro;

public partial class ProBroViewModel
{
    public async Task MoveRegPicUp(int photoID)
    {
        _dispatcherService.Invoke(() =>
        {
            foreach (var pix in Photos)
            {
                pix.PhotosEnabled = false;
            }
        });
        var wgore = Photos.First(p => p.PhotoId == photoID);
        var wdol = Photos.First(p => p.Position == wgore.Position - 1);
        await DatabaseAccessLayer.MovRegPhotoUp(photoID, SelectedItem.Id);
        await _activityTaskWrapper.ExecuteTaskAsync(DatabaseAccessLayer.RefreshBody(SelectedItem.Id, false), denLanguageResourses.Resources.RefreshingProductDetails);

        wgore.Position = wdol.Position;
        wdol.Position++;
        wgore.Photo = DatabaseAccessLayer.items[SelectedItem.Id].Photos.First(p => p.photoID == wgore.PhotoId);
        wgore.CanMoveUp = wgore.Photo.pos > 0;
        wdol.Photo = DatabaseAccessLayer.items[SelectedItem.Id].Photos.First(p => p.photoID == wdol.PhotoId);
        wdol.CanMoveUp = true;
        Photos = new ObservableCollection<PhotoViewModel>(Photos.OrderBy(p => p.Position));
        RecountPhotos();
        _dispatcherService.Invoke(() =>
        {
            foreach (var pix in Photos)
            {
                pix.PhotosEnabled = true;
            }
        });
    }
    public void RecountPhotos()
    {
        int i = 1;
        foreach (var pic in Photos)
        {
            pic.PicDesc = string.Format(denLanguageResourses.Resources.PhotoDescription, i, Photos.Count);
            i++;
        }
    }

    public async Task RemoveRegPic(string filename)
    {
        _dispatcherService.Invoke(() =>
        {
            foreach (var pix in Photos)
            {
                pix.PhotosEnabled = false;
            }
        });
        var ru = Photos.First(p => p.ImagePath == filename);
        Photos.Remove(ru);
        for (int i = 0; i < Photos.Count; i++)
        {
            var foto = Photos[i];
            foto.Photo = DatabaseAccessLayer.items[SelectedItem.Id].Photos[i];
            foto.Position = foto.Photo.pos;
            foto.PicDesc = string.Format(denLanguageResourses.Resources.PhotoDescription, i + 1, DatabaseAccessLayer.items[SelectedItem.Id].Photos.Count);
            foto.CanMoveUp = foto.Photo.pos > 0;
        }
        await PhotoActions.RemoveRegPhoto(SelectedItem.Id, filename);
        _dispatcherService.Invoke(RecountPhotos);

        await _activityTaskWrapper.ExecuteTaskAsync(DatabaseAccessLayer.RefreshBody(SelectedItem.Id, false), denLanguageResourses.Resources.RefreshingProductDetails);
        _dispatcherService.Invoke(() =>
        {
            foreach (var pix in Photos)
            {
                pix.PhotosEnabled = true;
            }
        });
    }

    private async Task ExecuteDeleteLogoPhoto()
    {
        LogoImage = null;
        await PhotoActions.RemoveLogoPhoto(DatabaseAccessLayer.items[SelectedItem.Id].itembody);
        await _activityTaskWrapper.ExecuteTaskAsync(DatabaseAccessLayer.RefreshBody(SelectedItem.Id, false), denLanguageResourses.Resources.RefreshingProductDetails);
    }

    private async Task ExecuteDeletePackagePhoto()
    {
        LogoImage = null;
        await PhotoActions.RemovePackagePhoto(DatabaseAccessLayer.items[SelectedItem.Id].itembody);
        await _activityTaskWrapper.ExecuteTaskAsync(DatabaseAccessLayer.RefreshBody(SelectedItem.Id, false), denLanguageResourses.Resources.RefreshingProductDetails);
    }

    private async Task SaveImageAsync(imgWithName imageSource)
    {
        var stream = await _fileDialogService.SaveFileAsync(denLanguageResourses.Resources.SaveImageFile, "JPEG Image|*.jpg|Png Image|*.png|Bitmap Image|*.bmp");

        if (stream is not null)
        {
            await using (stream)
            {
                await stream.WriteAsync(imageSource.pic, 0, imageSource.pic.Length);
            }
        }
    }

    public async Task DownloadLogoPhoto(CancellationToken ct)
    {
        var currentSelectedItem = SelectedItem;

        Progress<double> progressReport = null;
        _dispatcherService.Invoke(() =>
        {
            progressReport = new Progress<double>(percentage =>
            {
                LogoProgressBar = percentage;
            });

            IsLogoImageLoading = true;
            IsLogoImageLoaded = false;
        });


        var pix = await PhotoActions.getImageFromWeb4(DatabaseAccessLayer.items[currentSelectedItem.Id].itembody.logoPic, ct, progressReport);

        if (ct.IsCancellationRequested)
        {

            return;
        }

        var lo = new imgWithName
        {
            name = DatabaseAccessLayer.items[currentSelectedItem.Id].itembody.logoPic,
            pic = pix
        };


        _dispatcherService.Invoke(() =>
        {

            if (lo != null && currentSelectedItem == SelectedItem)
            {
                lo.itembodyid = currentSelectedItem.Id;
                LogoImage = lo;
            }

            IsLogoImageLoaded = true;
            IsLogoImageLoading = false;

        });
    }

    public async Task DownloadPackagePhoto(CancellationToken ct)
    {
        var currentSelectedItem = SelectedItem;

        Progress<double> progressReport = null;
        _dispatcherService.Invoke(() =>
        {
            progressReport = new Progress<double>(percentage =>
            {
                LogoProgressBar = percentage;
            });

            IsPackageImageLoading = true;
            IsPackageImageLoaded = false;
        });


        var pix = await PhotoActions.getImageFromWeb4(DatabaseAccessLayer.items[currentSelectedItem.Id].itembody.packagePic, ct, progressReport);

        if (ct.IsCancellationRequested)
        {

            return;
        }

        var lo = new imgWithName
        {
            name = DatabaseAccessLayer.items[currentSelectedItem.Id].itembody.packagePic,
            pic = pix
        };


        _dispatcherService.Invoke(() =>
        {

            if (lo != null && currentSelectedItem == SelectedItem)
            {
                lo.itembodyid = currentSelectedItem.Id;
                PackageImage = lo;
            }

            IsPackageImageLoaded = true;
            IsPackageImageLoading = false;

        });
    }

    public List<Task> DownloadMainPhotosIfNecessary(CancellationToken ct)
    {
        List<Task> PhotoTasks = new List<Task>();
        if (string.IsNullOrEmpty(DatabaseAccessLayer.items[SelectedItem.Id].itembody.logoPic))
        {
            LogoImage = null;
        }
        else
        {
            if (LogoImage == null || LogoImage.name != DatabaseAccessLayer.items[SelectedItem.Id].itembody.logoPic)
            {
                PhotoTasks.Add(DownloadLogoPhoto(ct));
            }

        }
        if (string.IsNullOrEmpty(DatabaseAccessLayer.items[SelectedItem.Id].itembody.packagePic))
        {
            PackageImage = null;
        }
        else
        {
            if (PackageImage == null || PackageImage.name != DatabaseAccessLayer.items[SelectedItem.Id].itembody.packagePic)
            {
                PhotoTasks.Add(DownloadPackagePhoto(ct));
            }
        }


        return PhotoTasks;
    }

    public async Task DownloadRegularPhotosIfNecessary(CancellationToken ct)
    {
        byte[][] fotony = null;
        photo[] photosNotInNames = null;

        if (!(DatabaseAccessLayer.items[SelectedItem.Id].Photos == null || DatabaseAccessLayer.items[SelectedItem.Id].Photos.Count == 0))
        {
            var names = Photos.Select(p => p.ImagePath).ToList();
            photosNotInNames = DatabaseAccessLayer.items[SelectedItem.Id].Photos
                .Where(p => !names.Contains(p.path))
                .OrderBy(p => p.pos)
                .ToArray();
            fotony = new byte[DatabaseAccessLayer.items[SelectedItem.Id].Photos.Count][];


        }
        else
        {
            _dispatcherService.Invoke(() => Photos.Clear());

            return;
        }

        var photosDict = Photos.ToDictionary(p => p.ImagePath, p => p);
        var allUpdatedPhotos = DatabaseAccessLayer.items[SelectedItem.Id].Photos.OrderBy(p => p.pos).ToArray();
        for (int i = 0; i < allUpdatedPhotos.Length; i++)
        {
            var updatedPhoto = allUpdatedPhotos[i];

            // Sprawdzanie, czy zdjęcie istnieje w oryginalnej kolekcji
            if (photosDict.TryGetValue(updatedPhoto.path, out var existingPhoto))
            {
                // Jeśli istnieje, kopiowanie byte[] do tablicy fotony
                fotony[i] = existingPhoto.Image;
            }
            else
            {
                // Jeśli nie istnieje, pozostawienie miejsca na nowe zdjęcie, które zostanie dodane później
                fotony[i] = null;
            }
        }




        var tasks = new Task<byte[]>[photosNotInNames.Length];
        for (int i = 0; i < photosNotInNames.Length; i++)
        {
            string path = photosNotInNames[i].path;
            tasks[i] = PhotoActions.getImageFromWeb4(path, ct);

        }

        byte[][] results = await Task.WhenAll(tasks);
        if (ct.IsCancellationRequested)
        {
            return;
        }
        for (int i = 0; i < photosNotInNames.Length; i++)
        {
            fotony[i] = results[i];
        }

        int j = 0;
        var ge = new List<PhotoViewModel>();
        foreach (var photo in DatabaseAccessLayer.items[SelectedItem.Id].Photos)
        {
            var photoViewModel = new PhotoViewModel(_fileDialogService)
            {
                Photo = photo,
                PhotoId = photo.photoID,
                Position = photo.pos,
                ImagePath = photo.path,
                PicDesc = string.Format(denLanguageResourses.Resources.PhotoDescription, j + 1, DatabaseAccessLayer.items[SelectedItem.Id].Photos.Count),
                DeleteCommand = new AsyncRelayCommand(async () => await RemoveRegPic(photo.path)),
                MoveUpCommand = new AsyncRelayCommand(async () => await MoveRegPicUp(photo.photoID)),
                CanMoveUp = photo.pos > 0,
                IsLoaded = true,
                IsLoading = false,
                Image = fotony[j]
            };
            ge.Add(photoViewModel);
            j++;
        }
        _dispatcherService.Invoke(() => Photos = new ObservableCollection<PhotoViewModel>(ge));


    }

    public async Task DownloadPhotosIfNecessary(CancellationToken ct)
    {
        var taski = DownloadMainPhotosIfNecessary(ct);
        taski.Add(DownloadRegularPhotosIfNecessary(ct));
        var mainTask = Task.WhenAll(taski);
        await _activityTaskWrapper.ExecuteTaskAsync(mainTask, denLanguageResourses.Resources.DownloadingPhotos, ct);
    }

    public async Task AddLogoPhoto()
    {
        var fileName = await _fileDialogService.OpenFileAsync(denLanguageResourses.Resources.LoadPictureWithLogo, "Png Image|*.png|Bitmap Image|*.bmp|JPEG Image|*.jpg");
        if (fileName != null)
        {
            string g = await _activityTaskWrapper.ExecuteTaskWithResultAsync(PhotoActions.UploadFileAsync(fileName, "logoPic", SelectedItem.Id), denLanguageResourses.Resources.UploadingLogoPhoto);
            LogoImage = new imgWithName
            {
                pic = await File.ReadAllBytesAsync(fileName),
                name = g,
                itembodyid = SelectedItem.Id
            };
            await _activityTaskWrapper.ExecuteTaskAsync(DatabaseAccessLayer.RefreshBody(SelectedItem.Id, false), denLanguageResourses.Resources.RefreshingProductDetails);

        }
    }

    public async Task AddPackagePhoto()
    {
        var fileName = await _fileDialogService.OpenFileAsync(denLanguageResourses.Resources.LoadPictureOfPackage, "Png Image|*.png|Bitmap Image|*.bmp|JPEG Image|*.jpg");
        if (fileName != null)
        {
            var g = await PhotoActions.UploadFileAsync(fileName, "packagePic", SelectedItem.Id);
            LogoImage = new imgWithName
            {
                pic = await File.ReadAllBytesAsync(fileName),
                name = g,
                itembodyid = SelectedItem.Id
            };
            await _activityTaskWrapper.ExecuteTaskAsync(DatabaseAccessLayer.RefreshBody(SelectedItem.Id, false), denLanguageResourses.Resources.RefreshingProductDetails);
        }
    }

    public async Task AddRegPhoto()
    {
        _dispatcherService.Invoke(() =>
        {
            foreach (var pix in Photos)
            {
                pix.PhotosEnabled = false;
            }
        });
        var fileName = await _fileDialogService.OpenFileAsync(denLanguageResourses.Resources.LoadPicture, "Png Image|*.png|Bitmap Image|*.bmp|JPEG Image|*.jpg");
        if (fileName != null)
        {
            string g = await _activityTaskWrapper.ExecuteTaskWithResultAsync(PhotoActions.UploadFileAsync(fileName, "regPic", SelectedItem.Id), denLanguageResourses.Resources.UploadingPhoto);
            await _activityTaskWrapper.ExecuteTaskAsync(DatabaseAccessLayer.RefreshBody(SelectedItem.Id, false), denLanguageResourses.Resources.RefreshingProductDetails);


            var photo = DatabaseAccessLayer.items[SelectedItem.Id].Photos.OrderBy(p => p.pos).Last();
            if (!_cts.IsCancellationRequested)
            {
                var photoViewModel = new PhotoViewModel(_fileDialogService)
                {
                    Image = await File.ReadAllBytesAsync(fileName),
                    Photo = photo,
                    PhotoId = photo.photoID,
                    Position = photo.pos,
                    ImagePath = photo.path,
                    PicDesc = string.Format(denLanguageResourses.Resources.PhotoDescription, DatabaseAccessLayer.items[SelectedItem.Id].Photos.Count, DatabaseAccessLayer.items[SelectedItem.Id].Photos.Count),
                    DeleteCommand = new AsyncRelayCommand(async () => await RemoveRegPic(photo.path)),
                    MoveUpCommand = new AsyncRelayCommand(async () => await MoveRegPicUp(photo.photoID)),
                    CanMoveUp = photo.pos > 0,
                    IsLoaded = true,
                    IsLoading = false
                };
                Photos.Add(photoViewModel);
                RecountPhotos();
            }
            await _activityTaskWrapper.ExecuteTaskAsync(DatabaseAccessLayer.RefreshBody(SelectedItem.Id, false), denLanguageResourses.Resources.RefreshingProductDetails);
        }
        _dispatcherService.Invoke(() =>
        {
            foreach (var pix in Photos)
            {
                pix.PhotosEnabled = true;
            }
        });
    }
}