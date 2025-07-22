namespace ColoursOperations;

public interface IColourOpsMediator
{
    byte[] ConvertToPlatformBitmap(byte[] pixelData);
    Task<byte[]> GetPixelsFromImageAsync(string imagePath);

}