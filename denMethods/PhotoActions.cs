using DataServicesNET80.Models;

namespace denMethods;

public static class PhotoActions
{
    public static async Task RemoveLogoPhoto(itembody itembody)
    {
        var multiForm = new MultipartFormDataContent();
        multiForm.Add(new StringContent(itembody.itembodyID.ToString()), "itemBodyID");
        multiForm.Add(new StringContent("logoPic"), "placeHolder");
        multiForm.Add(new StringContent(itembody.logoPic), "fileName");
        var url = "https://buchmiet.com/api/image/RemovePic";
        using (HttpClient client = new HttpClient())
        {
            var response = await client.PostAsync(url, multiForm).ConfigureAwait(false);
            var g = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
          
    }

    public static async Task RemovePackagePhoto(itembody itembody)
    {
        var multiForm = new MultipartFormDataContent
        {
            { new StringContent(itembody.itembodyID.ToString()), "itemBodyID" },
            { new StringContent("packagePic"), "placeHolder" },
            { new StringContent(itembody.packagePic), "fileName" }
        };
        var url = "https://buchmiet.com/api/image/RemovePic";
        using (HttpClient client = new())
        {
            var response = await client.PostAsync(url, multiForm).ConfigureAwait(false);
            var g = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
         
    }

    public static async Task RemoveRegPhoto(int itembodyid, string filename)
    {
        var multiForm = new MultipartFormDataContent
        {
            { new StringContent(itembodyid.ToString()), "itemBodyID" },
            { new StringContent("regPic"), "placeHolder" },
            { new StringContent(filename), "fileName" }
        };
        var url = "https://buchmiet.com/api/image/RemovePic";
        using (HttpClient client = new())
        {
            var response = await client.PostAsync(url, multiForm).ConfigureAwait(false);
            var g = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
        
    }

    public static async Task<string> UploadFileAsync(string path, string action, int itembodyID)
    {
        var multiForm = new MultipartFormDataContent();
        FileStream fs = File.OpenRead(path);
        multiForm.Add(new StreamContent(fs), "file", Path.GetFileName(path));
        multiForm.Add(new StringContent(itembodyID.ToString()), "itemBodyID");
        multiForm.Add(new StringContent(action), "placeHolder");
        // send request to API
        var url = "https://buchmiet.com/api/image/UploadNewPic";
        string g = "";
        using (HttpClient client = new())
        {
            var response = await client.PostAsync(url, multiForm).ConfigureAwait(false); ;
            g = await response.Content.ReadAsStringAsync().ConfigureAwait(false); ;
        }
        return g;

    }

    //public static async Task<imgWithName> getImageFromWeb5(string name, CancellationToken cancellationToken, IProgress<double> progress = null)
    //{
    //    if (string.IsNullOrEmpty(name))
    //    {
    //        return null;
    //    }

    //    var pic = new BitmapImage();

    //    using (HttpClient client = new HttpClient())
    //    {
    //        client.Timeout = TimeSpan.FromSeconds(3);
    //        MemoryStream ms = new MemoryStream();

    //        try
    //        {
    //            var response = await client.GetAsync($"https://productpictures.time4parts.co.uk/{name}", HttpCompletionOption.ResponseHeadersRead, cancellationToken);

    //            if (!response.IsSuccessStatusCode)
    //            {
    //                return null; // Brak odpowiedzi lub odpowiedź z błędem.
    //            }

    //            using (var stream = await response.Content.ReadAsStreamAsync())
    //            {
    //                var totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : 0;
    //                var bytesRead = 0L;
    //                var buffer = new byte[1024 * 16];
    //                var bytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

    //                while (bytes != 0)
    //                {
    //                    bytesRead += bytes;

    //                    // Raportowanie postępu.
    //                    if (totalBytes != 0)
    //                    {
    //                        progress?.Report((double)bytesRead / totalBytes * 100);
    //                    }

    //                    await ms.WriteAsync(buffer, 0, bytes, cancellationToken);
    //                    bytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
    //                }
    //            }
    //        }
    //        catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
    //        {
    //            return null;
    //        }

    //        pic.BeginInit();
    //        pic.CacheOption = BitmapCacheOption.OnLoad;
    //        ms.Seek(0, SeekOrigin.Begin);
    //        pic.StreamSource = ms;
    //        pic.EndInit();
    //        pic.Freeze();
    //        return new imgWithName { pic = pic, name = name };
    //    }
    //}


    public static async Task<byte[]> getImageFromWeb4(string name, CancellationToken cancellationToken, IProgress<double> progress = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }

        MemoryStream ms = new MemoryStream();
        using (HttpClient client = new HttpClient())
        {
            //            client.Timeout = TimeSpan.FromSeconds(3);
               

            try
            {
                var response = await client.GetAsync($"https://productpictures.time4parts.co.uk/{name}", HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    return null; // Brak odpowiedzi lub odpowiedź z błędem.
                }

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : 0;
                    var bytesRead = 0L;
                    var buffer = new byte[1024 * 16];
                    var bytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);

                    while (bytes != 0)
                    {
                        bytesRead += bytes;

                        // Raportowanie postępu.
                        if (totalBytes != 0)
                        {
                            progress?.Report((double)bytesRead / totalBytes * 100);
                        }

                        await ms.WriteAsync(buffer, 0, bytes, cancellationToken).ConfigureAwait(false); ;
                        bytes = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false); 
                    }
                }
            }
            catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
            {
                return null;
            }
             
        }

        return ms.ToArray();
    }

}