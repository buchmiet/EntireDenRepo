namespace denModels;

public class CN22PropertiesPack
{
    public PrintLines PrintLines { get; set; }
    public string NameOfTheSender { get; set; }
    public string NameOfTheOnlyOneProduct { get; set; }
    public string PriceForTheOnlyOneProduct { get; set; }
    public string WeightForTheOnlyOneProduct { get; set; }
    public DisplayedCurrency DisplayedCurrency { get; set; }
    public string SelectedContentType { get; set; }
    public bool UseSignatureFile { get; set; }
    public string SelectedFileWithSignature { get; set; }
    public SkiaSharp.SKBitmap SignatureFileBitmap { get; set; }
}