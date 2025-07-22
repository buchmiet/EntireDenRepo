namespace denSharedLibrary;

public interface IBoxLabelToImageByteArray
{
    byte[] GenerateImage(LabelNamePack LabelNamePack, LabelProperties LabelProperty);
    byte[] GenerateImage(string topSmallText, string centralLargeText, string centralSmallText, string subtitle, float width, float height, float largeFont, float lesserFont, float topFont, float bottomFont);
}