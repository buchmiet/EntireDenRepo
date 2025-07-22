using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ColoursOperations;

namespace denWPFSharedLibrary;

public class WpfColourOpsMediator : IColourOpsMediator
{

    public async Task<byte[]> GetPixelsFromImageAsync(string imagePath)
    {
         
        BitmapSource srcImage;
        using (FileStream fs = File.OpenRead(imagePath))
        {
            srcImage = BitmapDecoder.Create(fs,BitmapCreateOptions.None, BitmapCacheOption.OnLoad).Frames[0];
        }

        if (srcImage.Format != PixelFormats.Bgra32)
        {
            srcImage = new FormatConvertedBitmap(srcImage, PixelFormats.Bgra32, null, 0);
        }
        var bmp=new WriteableBitmap(srcImage);
        var bufor = new byte[128 * 128 * 4];
        bmp.CopyPixels(bufor, 512, 0);                    
        return bufor;         
    }

    public byte[] ConvertToPlatformBitmap(byte[] pixelData)
    {
            
        int width = 128; 
        int height = 128;
        int stride = width * 4;
        using (MemoryStream stream = new MemoryStream())
        {
            var pix = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixelData, stride);
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(pix));
            encoder.Save(stream);
            return stream.ToArray();
        }
    }

       
}