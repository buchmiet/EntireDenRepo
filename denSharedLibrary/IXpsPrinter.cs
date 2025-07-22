namespace denSharedLibrary;

public interface IXpsPrinter
{
    void SetPrinter(string printerName);
    void Print(Stream InputStream);
       
}