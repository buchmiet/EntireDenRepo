
using System.Drawing;
using System.Drawing.Printing;
using DataServicesNET80.Models;
using denMethods;
using denModels;
using denSharedLibrary;
using SettingsKeptInFile;
using SkiaSharp;

namespace Printers;

public interface IPrintersService
{
    PaperSize ConvertMillimetersToPaperSize(int widthInMillimeters, int heightInMillimeters);
    byte[] CreateCN22Image(CN22PropertiesPack properties, Complete komplet);
    CN22PropertiesPack PrepareCN22Fields();
    void PrintLabelFromItemBody(LabelProperties labelProperties, string printerName, short copies, itembody _itemBody, multidrawer _multiDrawer, bodyinthebox _bodyInTheBox);
    void PrintBWLabel(LabelProperties labelProperties, LabelNamePack LabelNamePack, string printerName, short copies);
    void PrintLabel(LabelProperties labelProperties, string printerName, short copies, byte[] imageStream);
    void PrintLabel(LabelProperties labelProperties, LabelNamePack LabelNamePack, string printerName, short copies, IBoxLabelToImageByteArray boxLabelToImageByteArray);
    void PrintAddressLabel(int width, int heigh, string printerName, short copies, byte[] addressLabelStream);
    void PrintCN22s(List<Complete> komplety, string printerName);
    void Print4x6Label(string printerName, short copies, byte[] cn22LabelStream);
    KeyValuePair<string, List<string>> GetPrinters(string printerType);
}

public class PrintersService : IPrintersService
{
    private const float Dpi = 72;

    private ISettingsService SettingsService { get; }
    public PrintersService(ISettingsService settingsService)
    {
        SettingsService = settingsService;
        CN22FilePath = Path.Combine(AppContext.BaseDirectory, @"Data\cn22.png");
    }

    private readonly string CN22FilePath;// => Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Data\cn22.png");

    public PaperSize ConvertMillimetersToPaperSize(int widthInMillimeters, int heightInMillimeters)
    {
        int widthInHundredthsOfInch = Convert.ToInt32((widthInMillimeters / 25.4) * 100);
        int heightInHundredthsOfInch = Convert.ToInt32((heightInMillimeters / 25.4) * 100);

        return new PaperSize("Custom Size", widthInHundredthsOfInch, heightInHundredthsOfInch);
    }

    public byte[] CreateCN22Image(CN22PropertiesPack properties, Complete komplet)
    {
        if (properties.NameOfTheSender == null) return null;
        using var stream = new FileStream(CN22FilePath, FileMode.Open);
        using var bitmap = SKBitmap.Decode(stream);
        using var canvas = new SKCanvas(bitmap);
        {
            AddTextToCanvas(canvas, 284, 331, 925, 375, properties.NameOfTheSender, 40);
            var adr = SettingsService.GetSetting("businessaddress");//.GetValue<string>();
            if (adr.IsSuccess)
            {
                var BusinessAddress = Base64Converter.DecodeBase64ToString(adr.GetValue<string>(). Trim());
                AddTextToCanvas(canvas, 290, 400, 947, 611, BusinessAddress, 50);
            }

            switch (properties.SelectedContentType)
            {
                case "Gift":
                    AddTextToCanvas(canvas, 325, 634, 368, 660, "X", 60);
                    break;
                case "Documents":
                    AddTextToCanvas(canvas, 325, 673, 368, 701, "X", 60);
                    break;
                case "Sale of Goods":
                    AddTextToCanvas(canvas, 325, 746, 368, 747, "X", 60);
                    break;
                case "Commercial Sample":
                    AddTextToCanvas(canvas, 601, 634, 644, 660, "X", 60);
                    break;
                case "Returned Goods":
                    AddTextToCanvas(canvas, 601, 673, 644, 701, "X", 60);
                    break;
            }

            string currency = "";
            string cursymbol = "";

            if (properties.DisplayedCurrency == DisplayedCurrency.OrderCurrency)
            {
                currency = komplet.Order.salecurrency;
            }
            else
            {
                if (SettingsService.GetSetting("currency") != null)
                {
                    currency = SettingsService.GetSetting("currency").GetValue<string>();
                }
                else
                {
                    currency = "GBP";
                    SettingsService.UpdateSetting("currency", "GBP");
                }
            }

            switch (currency)
            {
                case "GBP":
                    cursymbol = "£";
                    break;
                case "EUR":
                    cursymbol = "€";
                    break;
                case "USD":
                    cursymbol = "$";
                    break;
            }


            if (properties.PrintLines == PrintLines.ForOneProduct)
            {
                OneProductToCanvas(canvas, cursymbol);
            }
            else
            {
                SKBitmap productLines;
                byte[] bitmpArray;
                try
                {
                    var o = new OrderItemsToImageByteArray();
                    bitmpArray = o.GenerateStream(komplet.OrderItems, cursymbol);
                    using (var bitmapstream = new MemoryStream(bitmpArray))
                    {
                        AddBitmapToCanvas(canvas, 38, 808, 973, 1035, SKBitmap.Decode(bitmapstream));
                    }

                    int totalQuantity = komplet.OrderItems.Sum(item => item.quantity);
                    int totalWeight = komplet.OrderItems.Sum(item =>
                        item.ItemWeight.HasValue ? item.ItemWeight.Value * item.quantity : 0);
                    decimal totalPrice = komplet.OrderItems.Sum(item => item.price * item.quantity);

                    AddTextToCanvas(canvas, 571, 1048, totalQuantity.ToString(), 35);
                    AddTextToCanvas(canvas, 713, 1048, totalWeight.ToString(), 35);
                    AddTextToCanvas(canvas, 860, 1048, cursymbol + Math.Round(totalPrice, 2).ToString(), 35);
                }
                catch (Exception ex)
                {
                    OneProductToCanvas(canvas, cursymbol);
                }
            }

            AddTextToCanvas(canvas, 167, 1381, DateTime.Today.ToString("yyyy/MM/dd"), 40);

            if (properties.UseSignatureFile && properties.SignatureFileBitmap != null)
            {
                AddBitmapToCanvas(canvas, 455, 1385, 855, 1438, properties.SignatureFileBitmap);
            }
        }

        using (var memoryStream = new MemoryStream())
        {
            bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(memoryStream);
            return memoryStream.ToArray();
        }

        void OneProductToCanvas(SKCanvas canvas, string cursymbol)
        {
            AddTextToCanvas(canvas, 48, 810, properties.NameOfTheOnlyOneProduct, 40);
            AddTextToCanvas(canvas, 571, 810, "1", 40);
            AddTextToCanvas(canvas, 713, 810, properties.WeightForTheOnlyOneProduct, 40);
            AddTextToCanvas(canvas, 860, 810, cursymbol + properties.PriceForTheOnlyOneProduct, 40);

            AddTextToCanvas(canvas, 571, 1048, "1", 40);
            AddTextToCanvas(canvas, 713, 1048, properties.WeightForTheOnlyOneProduct, 40);
            AddTextToCanvas(canvas, 860, 1048, cursymbol + properties.PriceForTheOnlyOneProduct, 40);
        }
    }


    private void AddBitmapToCanvas(SKCanvas canvas, float x, float y, float xmax, float ymax, SKBitmap extraImage)
    {


        using (var paint = new SKPaint())
        {
            var rect = new SKRect(x, y, xmax, ymax);
            canvas.DrawBitmap(extraImage, rect, paint);
        }
    }

    private  void AddTextToCanvas(SKCanvas canvas, float x, float y, float xmax, float ymax, string text, float fontSize)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }
        var paint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = fontSize,
            Typeface = SKTypeface.FromFamilyName("Calibri"),
            IsStroke = false,
            StrokeWidth = 0
        };

        float maxWidth = xmax - x;
        float maxHeight = ymax - y;
        float lineSpacing = 5;

        string[] lines = text.Split([Environment.NewLine], StringSplitOptions.None);

        float maxLineWidth = 0;
        float totalHeight = 0;
        List<SKPath> textPaths = new List<SKPath>();
        foreach (var line in lines)
        {
            var path = paint.GetTextPath(line, 0, 0);
            textPaths.Add(path);

            var bounds = path.Bounds;
            maxLineWidth = Math.Max(maxLineWidth, bounds.Width);
            totalHeight += bounds.Height + lineSpacing;
        }

        totalHeight -= lineSpacing;

        float scaleX = maxWidth / maxLineWidth;
        float scaleY = maxHeight / totalHeight;
        float scale = Math.Min(scaleX, scaleY);

        for (int i = 0; i < textPaths.Count; i++)
        {
            var matrix = SKMatrix.CreateScale(scale, scale);
            textPaths[i].Transform(matrix);
        }

        totalHeight *= scale;
        float centeringOffset = (maxHeight - totalHeight) / 2;  // Obliczenie przesunięcia do wyśrodkowania tekstu

        float startHeight = y + centeringOffset;
        foreach (var path in textPaths)
        {
            var bounds = path.Bounds;
            var matrix = SKMatrix.CreateTranslation(x, startHeight - bounds.Top);
            path.Transform(matrix);

            canvas.DrawPath(path, paint);

            startHeight += bounds.Height + lineSpacing * scale;
        }
    }

    private   void AddTextToCanvas(SKCanvas canvas, float x, float y, string text, float fontSize)
    {
        if (canvas == null) throw new ArgumentNullException(nameof(canvas));
        if (string.IsNullOrEmpty(text)) return;

        using (var paint = new SKPaint())
        {
            paint.TextSize = fontSize;
            paint.IsAntialias = true;
            paint.Color = SKColors.Black; // możesz zmienić kolor według potrzeb

            // Ustalamy współrzędne początkowe, aby górny lewy róg tekstu był w pozycji (x, y)
            float textWidth = paint.MeasureText(text);
            SKRect textBounds = new SKRect();
            paint.MeasureText(text, ref textBounds);
            float textHeight = textBounds.Height;

            canvas.DrawText(text, x - textBounds.Left, y + textHeight, paint);
        }
    }


    public   CN22PropertiesPack PrepareCN22Fields()
    {
        var propertiesPack = new CN22PropertiesPack();

        // PrintLines
        var printLines = SettingsService.GetSetting("cn22productsshown");
        if (!printLines.IsSuccess)
        {
            SettingsService.UpdateSetting("cn22productsshown", "oneproduct");
            propertiesPack.PrintLines = PrintLines.ForOneProduct;
        }
        else
        {
            propertiesPack.PrintLines = printLines.GetValue<string>() switch
            {
                "allproducts" => PrintLines.ForAllSoldProducts,
                _ => PrintLines.ForOneProduct
            };
        }

        // NameOfTheSender
        var senderNameSetting = SettingsService.GetSetting("cn22nameofthesender");
        if (!senderNameSetting.IsSuccess)
        {
            SettingsService.UpdateSetting("cn22nameofthesender", "Sender's name");
            propertiesPack.NameOfTheSender = "Sender's name";
        }
        else
        {
            propertiesPack.NameOfTheSender = senderNameSetting.GetValue<string>();
        }

        // NameOfTheOnlyOneProduct
        var productNameSetting = SettingsService.GetSetting("cn22nameofoneproduct");
        if (!productNameSetting.IsSuccess)
        {
            // Brak ustawienia - ustaw domyślną wartość i zapisz
            propertiesPack.NameOfTheOnlyOneProduct = "Product";
            SettingsService.UpdateSetting("cn22nameofoneproduct", "Product");
        }
        else
        {
            // Pobierz i użyj istniejącej wartości (gwarantowane nie-null)
            propertiesPack.NameOfTheOnlyOneProduct = productNameSetting.GetValue<string>();
        }

        // PriceForTheOnlyOneProduct
        var priceSetting = SettingsService.GetSetting("cn22priceofoneproduct");
        if (!priceSetting.IsSuccess)
        {
            // Brak ustawienia - ustaw domyślną cenę i zapisz
            propertiesPack.PriceForTheOnlyOneProduct = "6.99";
            SettingsService.UpdateSetting("cn22priceofoneproduct", "6.99");
        }
        else
        {
            // Pobierz istniejącą wartość ceny (gwarantowane nie-null)
            propertiesPack.PriceForTheOnlyOneProduct = priceSetting.GetValue<string>();
        }

        // WeightForTheOnlyOneProduct
        var weightSetting = SettingsService.GetSetting("cn22weightofoneproduct");
        if (!weightSetting.IsSuccess)
        {
            // Brak ustawienia - ustaw domyślną wagę i zapisz
            propertiesPack.WeightForTheOnlyOneProduct = "20g";
            SettingsService.UpdateSetting("cn22weightofoneproduct", "20g");
        }
        else
        {
            // Użyj istniejącej wartości (gwarantowana nie-nullowość)
            propertiesPack.WeightForTheOnlyOneProduct = weightSetting.GetValue<string>();
        }

        // DisplayedCurrency
        var currencySetting = SettingsService.GetSetting("cn22currency");
        if (!currencySetting.IsSuccess)
        {
            // Brak ustawienia - ustaw domyślną walutę i zapisz wartość "app"
            propertiesPack.DisplayedCurrency = DisplayedCurrency.MainCurrency;
            SettingsService.UpdateSetting("cn22currency", "app");
        }
        else
        {
            // Obsłuż istniejące ustawienie zgodnie z logiką switch
            var currencyValue = currencySetting.GetValue<string>();
            propertiesPack.DisplayedCurrency = currencyValue switch
            {
                "order" => DisplayedCurrency.OrderCurrency,
                _ => DisplayedCurrency.MainCurrency
            };
        }

        // SelectedContentType
        var contentTypeSetting = SettingsService.GetSetting("cn22contenttype");
        var allowedContentTypes = new[] { "Gift", "Documents", "Sale of Goods", "Commercial Sample", "Returned Goods" };

        if (!contentTypeSetting.IsSuccess)
        {
            // Brak ustawienia - zainicjuj domyślną wartość
            propertiesPack.SelectedContentType = "Gift";
            SettingsService.UpdateSetting("cn22contenttype", "Gift");
        }
        else
        {
            var contentTypeValue = contentTypeSetting.GetValue<string>();

            if (allowedContentTypes.Contains(contentTypeValue))
            {
                // Poprawna wartość - użyj istniejącego ustawienia
                propertiesPack.SelectedContentType = contentTypeValue;
            }
            else
            {
                // Nieprawidłowa wartość - nadpisz i ustaw domyślną
                propertiesPack.SelectedContentType = "Gift";
                SettingsService.UpdateSetting("cn22contenttype", "Gift");
            }
        }

        // UseSignatureFile and SignatureFileBitmap
        var useSignatureSetting = SettingsService.GetSetting("cn22usesignaturefile");
        propertiesPack.UseSignatureFile = useSignatureSetting.IsSuccess &&
                                          useSignatureSetting.GetValue<string>() == "true";

        if (propertiesPack.UseSignatureFile)
        {
            var signatureFileSetting = SettingsService.GetSetting("cn22signaturefile");

            if (!signatureFileSetting.IsSuccess)
            {
                // Brak pliku podpisu mimo włączonej opcji - wyłącz funkcjonalność
                propertiesPack.UseSignatureFile = false;
                SettingsService.UpdateSetting("cn22usesignaturefile", "false");
                return propertiesPack;
            }

            try
            {
                var signatureFilePath = signatureFileSetting.GetValue<string>();
                using var stream = new FileStream(signatureFilePath, FileMode.Open);
                propertiesPack.SignatureFileBitmap = SKBitmap.Decode(stream);
                propertiesPack.SelectedFileWithSignature = signatureFilePath;
            }
            catch (Exception ex) when (ex is FileNotFoundException ||
                                       ex is IOException )
            {
                // Błąd ładowania pliku - wyczyść wartości i zgłoś błąd
                propertiesPack.SignatureFileBitmap?.Dispose();
                propertiesPack.SignatureFileBitmap = null;
                propertiesPack.SelectedFileWithSignature = null;
                return null;
            }
        }

        return propertiesPack;
    }



    public   void PrintLabelFromItemBody(LabelProperties labelProperties, string printerName, short copies, itembody _itemBody, multidrawer _multiDrawer, bodyinthebox _bodyInTheBox)
    {
        var myname = _itemBody.myname;
        if (myname.Contains("\\"))
        {
            myname = myname.Substring(0, myname.IndexOf("\\"));
        }
    }

    public   void PrintBWLabel(LabelProperties labelProperties, LabelNamePack LabelNamePack, string printerName, short copies)
    {
        int ImageWidth = Convert.ToInt32(((labelProperties.Width / 25.4) * Dpi));
        int ImageHeight = Convert.ToInt32(((labelProperties.Height / 25.4) * Dpi));
        var _image = LabelPropertiesManager.PlaceStringsOnCanvas(null, null, null, null, labelProperties, ImageWidth * 4, ImageHeight * 4, LabelNamePack, false, 4);
        PrintLabel(labelProperties, printerName, copies, _image);
    }


    public void PrintLabel(LabelProperties labelProperties, string printerName, short copies, byte[] imageStream)
    {
        PrintDocument printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = printerName;
        printDocument.DefaultPageSettings.Landscape = labelProperties.Landscape;
        printDocument.PrinterSettings.Copies = copies;

        int width;
        int height;
        if (labelProperties.Landscape)
        {
            width = labelProperties.Width;
            height = labelProperties.Height;
        }
        else
        {
            width = labelProperties.Height;
            height = labelProperties.Width;
        }

        printDocument.DefaultPageSettings.PaperSize = ConvertMillimetersToPaperSize(width, height);
        printDocument.DefaultPageSettings.Landscape = !labelProperties.Landscape;
        printDocument.PrintPage += (sender, e) =>
        {
            using MemoryStream ms = new MemoryStream(imageStream);
            using System.Drawing.Image image = Image.FromStream(ms);
            image.Save(@"c:\buchmiet.ltd\tescik.jpg");
            var printableArea = e.PageSettings.PrintableArea;
            if (labelProperties.Landscape)
            {
                e.Graphics.DrawImage(image,
                    new Rectangle((int)printableArea.X, (int)printableArea.Y,
                        (int)(printableArea.Width - printableArea.X),
                        (int)(printableArea.Height - printableArea.Y)));
            }
            else
            {
                e.Graphics.DrawImage(image,
                    new Rectangle((int)printableArea.Y, (int)printableArea.X,
                        (int)(printableArea.Height - printableArea.Y),
                        (int)(printableArea.Width - printableArea.X)));
            }
        };
        // TODO 
        // why is it requiring buchmiet.ltd/tescik.jpg????
        if (printDocument != null)
        {
            try
            {
                printDocument.Print();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public   void PrintAddressLabel(int width, int heigh, string printerName, short copies, byte[] addressLabelStream)
    {
        PrintDocument printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = printerName;
        printDocument.DefaultPageSettings.Landscape = false;
        printDocument.PrinterSettings.Copies = copies;

        printDocument.DefaultPageSettings.PaperSize = ConvertMillimetersToPaperSize(width, heigh);
        printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
        printDocument.PrintPage += (sender, e) =>
        {
            using (MemoryStream ms = new MemoryStream(addressLabelStream))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
                {

                    e.Graphics.DrawImage(image, e.MarginBounds);
                }
            }
        };
        printDocument.Print();
    }

    public   void PrintCN22s(List<Complete> komplety, string printerName)
    {
        var cn22properties = PrepareCN22Fields();
        foreach (var k in komplety)
        {
            Print4x6Label(printerName, 1, CreateCN22Image(cn22properties, k));
        }

    }

    public   void Print4x6Label(string printerName, short copies, byte[] cn22LabelStream)
    {
        PrintDocument printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = printerName;
        printDocument.DefaultPageSettings.Landscape = false;
        printDocument.PrinterSettings.Copies = copies;

        printDocument.DefaultPageSettings.PaperSize = ConvertMillimetersToPaperSize(100, 160);
        printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 50);
        printDocument.PrintPage += (sender, e) =>
        {
            using (MemoryStream ms = new MemoryStream(cn22LabelStream))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
                {

                    e.Graphics.DrawImage(image, e.MarginBounds);
                }
            }
        };
        printDocument.Print();
    }


    public   void PrintLabel(LabelProperties labelProperties, LabelNamePack LabelNamePack, string printerName, short copies, IBoxLabelToImageByteArray boxLabelToImageByteArray)
    {
        var imageStream = boxLabelToImageByteArray.GenerateImage(LabelNamePack, labelProperties);

        PrintDocument printDocument = new PrintDocument();
        printDocument.PrinterSettings.PrinterName = printerName;
        printDocument.DefaultPageSettings.Landscape = labelProperties.Landscape;
        printDocument.PrinterSettings.Copies = copies;

        int width;
        int height;
        if (labelProperties.Landscape)
        {
            width = labelProperties.Width;
            height = labelProperties.Height;
        }
        else
        {
            width = labelProperties.Height;
            height = labelProperties.Width;
        }
        printDocument.DefaultPageSettings.PaperSize = ConvertMillimetersToPaperSize(width, height);
        printDocument.PrintPage += (sender, e) =>
        {
            using (MemoryStream ms = new MemoryStream(imageStream))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromStream(ms))
                {
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    e.Graphics?.DrawImage(image, e.PageBounds);
                }
            }
        };
        printDocument.Print();
    }

    public   KeyValuePair<string, List<string>> GetPrinters(string printerType)
    {
        var drukarki = new List<string>();

        for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)
        {
            var pkInstalledPrinters = System.Drawing.Printing.PrinterSettings.InstalledPrinters[i];
            drukarki.Add(pkInstalledPrinters);
        }
        if (drukarki.Count == 0)
        {
            return new KeyValuePair<string, List<string>>("", drukarki);
        }

        var MainSettings = SettingsService.GetAllSettings();

        if (MainSettings.ContainsKey(printerType))
        {
            string defaultPrinter = drukarki.FirstOrDefault(p => p.Equals(MainSettings[printerType]));
            if (defaultPrinter != null)
            {
                return new KeyValuePair<string, List<string>>(defaultPrinter, drukarki);
            }
        }
        return new KeyValuePair<string, List<string>>(null, drukarki);
    }
}