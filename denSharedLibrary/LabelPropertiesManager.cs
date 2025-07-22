using System.Drawing.Text;
using DataServicesNET80;
using System.Reflection;
using System.Xml.Serialization;
using static denSharedLibrary.Colours;
using DataServicesNET80.Extensions;
using DataServicesNET80.Models;
using DataServicesNET80.DatabaseAccessLayer;
using HtmlAgilityPack;
using SkiaSharp;

namespace denSharedLibrary;

public static class LabelPropertiesManager
{

       

    public static byte[] PlaceStringsOnCanvas(RGB BottomTextColour,RGB TopTextColour,RGB CentralLargeTextColour, RGB CentralSmallTextColour,LabelProperties _labelProperties,int ImageWidth,int ImageHeight,LabelNamePack _labelNamePack,bool _colours,int? _quality)
    {
        int quality = _quality ?? 1;
        if (ImageHeight == 0 || ImageWidth == 0 || quality <= 0|| _labelProperties==null)
        {
            return null;
        }

        ImageWidth *= quality;
        ImageHeight *= quality;

        var labelProperties = new LabelProperties()
        {
            TopFont = _labelProperties.TopFont * quality,
            BottomFont = _labelProperties.BottomFont * quality,
            LargeFont = _labelProperties.LargeFont * quality,
            LesserFont = _labelProperties.LesserFont * quality,
            TopMargin = _labelProperties.TopMargin * quality,
            BottomMargin = _labelProperties.BottomMargin * quality,
            CentralLineSpacing = _labelProperties.CentralLineSpacing * quality,
        };

        SKBitmap bitmap = new SKBitmap(Convert.ToInt32(ImageWidth), Convert.ToInt32(ImageHeight));
        SKCanvas canvas = new SKCanvas(bitmap);
        canvas.Clear(SKColors.White);

        SKTypeface typeface = SKTypeface.FromFamilyName("Calibri");

        canvas.Scale(quality);  // Skalowanie całego płótna przed rysowaniem tekstu

        string topText = _labelNamePack.Toptext;
        SKColor topTextColor = _colours ? new SKColor(TopTextColour.R, TopTextColour.G, TopTextColour.B) : SKColors.Black;
        SKPaint topPaint = new SKPaint
        {
            Typeface = typeface,
            Color = topTextColor,
            TextSize = (float)labelProperties.TopFont,
            IsAntialias = true,
        };

        SKRect topSize = new SKRect();
        topPaint.MeasureText(topText, ref topSize);

        float topX = ((bitmap.Width / quality) - topSize.Width) / 2;
        float topY = (float)labelProperties.TopMargin + topSize.Height;

        canvas.DrawText(topText, topX, topY, topPaint);

        string bottomText = _labelNamePack.Bottomtext;
        SKColor bottomTextColor = _colours ? new SKColor(BottomTextColour.R, BottomTextColour.G, BottomTextColour.B) : SKColors.Black;
        SKPaint bottomPaint = new SKPaint
        {
            Typeface = typeface,
            Color = bottomTextColor,
            TextSize = (float)labelProperties.BottomFont,
            IsAntialias = true,
        };

        SKRect bottomSize = new SKRect();
        bottomPaint.MeasureText(bottomText, ref bottomSize);

        float bottomX = ((bitmap.Width / quality) - bottomSize.Width) / 2;
        float bottomY = (float)(bitmap.Height / quality) - (float)labelProperties.BottomMargin;

        canvas.DrawText(bottomText, bottomX, bottomY, bottomPaint);

        string text1 = _labelNamePack.CentralLargeText;
        SKColor text1Color = _colours ? new SKColor(CentralLargeTextColour.R, CentralLargeTextColour.G, CentralLargeTextColour.B) : SKColors.Black;
        SKPaint paint1 = new SKPaint
        {
            Typeface = typeface,
            Color = text1Color,
            TextSize = (float)labelProperties.LargeFont,
            IsAntialias = true,
            SubpixelText = true,
            LcdRenderText = true
        };

        string text2 = _labelNamePack.CentralSmallText;
        SKColor text2Color = _colours ? new SKColor(CentralSmallTextColour.R, CentralSmallTextColour.G, CentralSmallTextColour.B) : SKColors.Black;
        SKPaint paint2 = new SKPaint
        {
            Typeface = typeface,
            Color = text2Color,
            TextSize = (float)labelProperties.LesserFont,
            IsAntialias = true,
            SubpixelText = true,
            LcdRenderText = true
        };

        SKRect size1 = new SKRect();
        SKRect size2 = new SKRect();
        paint1.MeasureText(text1, ref size1);
        paint2.MeasureText(text2, ref size2);

        float totalHeight = (float)size1.Height + (float)size2.Height + (float)labelProperties.CentralLineSpacing;

        float y = ((bitmap.Height / quality) - totalHeight) / 2;

        float x1 = ((bitmap.Width / quality) - size1.Width) / 2;

        float x2 = ((bitmap.Width / quality) - size2.Width) / 2;

        canvas.DrawText(text1, x1, y + size1.Height, paint1);

        canvas.DrawText(text2, x2, (float)y + size1.Height + (float)labelProperties.CentralLineSpacing + size2.Height, paint2);

        canvas.Scale(1f / quality);  // Przywrócenie skalowania płótna po zakończeniu rysowania tekstu

        byte[] pngBytes;
        using (SKImage image = SKImage.FromBitmap(bitmap))
        {
            using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                pngBytes = data.ToArray();
            }
        }

        return pngBytes;
    }







    private static Lazy<List<LabelProperties>> LazyLabelProperties = new(LoadLabelProperties);
    private static string FilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\LabelProperties.xml");

    public static List<LabelProperties> GetLabelProperties()
    {
        return LazyLabelProperties.Value;
    }

    public static LabelProperties GetProperty(string labelName)
    {
        return LazyLabelProperties.Value.FirstOrDefault(lp => lp.LabelName == labelName);
    }

    private static List<LabelProperties> LoadLabelProperties()
    {
        if (File.Exists(FilePath))
        {
            using var stream = new FileStream(FilePath, FileMode.Open);
            var serializer = new XmlSerializer(typeof(List<LabelProperties>));
            return (List<LabelProperties>)serializer.Deserialize(stream);
        }
        return new List<LabelProperties>();
    }

    public static void SaveLabelProperties(LabelProperties updatedLabelProperties)
    {
        var labelPropertiesList = LazyLabelProperties.Value;
        var existingLabelProperty = labelPropertiesList.FirstOrDefault(l => l.LabelName == updatedLabelProperties.LabelName);
        if (existingLabelProperty != null)
        {
            var index = labelPropertiesList.IndexOf(existingLabelProperty);
            labelPropertiesList[index] = updatedLabelProperties;
        }
        else
        {
            labelPropertiesList.Add(updatedLabelProperties);
        }

        using var stream = new FileStream(FilePath, FileMode.Create);
        var serializer = new XmlSerializer(typeof(List<LabelProperties>));
        serializer.Serialize(stream, labelPropertiesList);
    }

    public static async Task<LabelNamePack> GetLabelNamePack(IDatabaseAccessLayer accessLayer, int itembodyid)
    {
        LabelNamePack zwrotka = new LabelNamePack();
        var body = accessLayer.items[itembodyid].itembody;
        zwrotka.CentralSmallText = body.myname;
        if (body.myname.Contains("\\"))
        {
            zwrotka.CentralSmallText = zwrotka.CentralSmallText.Substring(0, body.myname.IndexOf("\\"));
        }
        if (accessLayer.items[itembodyid].bodyinthebox != null)
        {
            var bb = accessLayer.items[itembodyid].bodyinthebox;
            var md = (await accessLayer.multidrawer()).First(p => p.MultiDrawerID == bb.MultiDrawerID);
            zwrotka.Toptext = md.name;
            zwrotka.CentralLargeText = ((char)(65 + bb.column)).ToString() + (bb.row + 1).ToString();
            zwrotka.Bottomtext = body.mpn;
            return zwrotka;
        }
        zwrotka.Toptext = "";
        zwrotka.CentralLargeText = zwrotka.CentralSmallText;
        zwrotka.CentralSmallText = "";
        zwrotka.Bottomtext = "";
        return zwrotka;
    }
     
    public static bool RemoveLabelProperties(string name)
    {
        var labelPropertiesList = LazyLabelProperties.Value;
        var denat = labelPropertiesList.First(p => p.LabelName.Equals(name));
        if (denat != null)
        {
            labelPropertiesList.Remove(denat);
            LazyLabelProperties = new Lazy<List<LabelProperties>>(() => labelPropertiesList);
            using var stream = new FileStream(FilePath, FileMode.Create);
            var serializer = new XmlSerializer(typeof(List<LabelProperties>));
            serializer.Serialize(stream, labelPropertiesList);
            return true;
        }
        return false;
    }

}


public static class ThemesActions
{
    private static List<Theme> _themes;

    public static List<Theme> MYThemes
    {
        get
        {
            if (_themes != null)
            {
                return _themes;
            }

            LoadThemesFromXml(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\ConsoleThemes.xml"));
            return _themes;
        }

    }

    public static Dictionary<string, string> FileName2FamilyName = new();

    private static void LoadThemesFromXml(string xmlFilePath)
    {

        List<Theme> themes = [];
        try
        {
              
            var serializer = new XmlSerializer(typeof(Themes));

            using (StreamReader reader = new StreamReader(xmlFilePath))
            {
                Themes themesContainer = (Themes) serializer.Deserialize(reader);
                themes = themesContainer.ThemesList;
            }
        } catch (Exception ex)
        {
            Console.WriteLine("Error loading themes from XML: " + ex.Message);
        }



        using (
            System.Drawing.Text.PrivateFontCollection _fontCollection = new PrivateFontCollection())
        {
            var oldnames=new List<string>();
            foreach (var theme in themes)
            {
                var fontFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\", theme.FontFile);             
                _fontCollection.AddFontFile(fontFilePath);                
                if (!FileName2FamilyName.ContainsKey(theme.FontFile))
                {
                    foreach (var ff in _fontCollection.Families.Select(p => p.Name))
                    {
                        if (!oldnames.Contains(ff))
                        {
                            FileName2FamilyName.Add(theme.FontFile, ff);
                            oldnames.Add(ff);
                        }
                    }
                }
            }
        }
           
        _themes = themes;
    }      
}


   

   
public delegate Task AsyncEventHandler(object sender, EventArgs e);

public interface IAsyncDialogViewModel
{
    event AsyncEventHandler RequestClose;
}

public interface IDialogViewModel
{
    event EventHandler RequestClose;
}


public static class CasioInteractions
{
    //public static async Task<bool> NLAGokanCheck(string mpn, IHttpClientFactory httpClientFactory, CancellationToken token = default(CancellationToken))
    //{
    //    try
    //    {
    //        using HttpClient client = httpClientFactory.CreateClient("NLOGokan");
    //        var builder = new UriBuilder("https://www.servicecasio.com/web/newpf/gokan.php")
    //        {
    //            Query = $"pcd={Uri.EscapeDataString(mpn)}&partscode=10557018&C1=1&C2=1&C3=1&C4=1&C5=1&C6=0&C7=0&C8=0&BRK=BRANK#"
    //        };

    //        string htmlCode = await client.GetStringAsync(builder.Uri, token);
    //        HtmlDocument doc = new HtmlDocument();
    //        doc.LoadHtml(htmlCode);
    //        var HTMLTableTRList = doc.DocumentNode.SelectSingleNode("//table//tr[14]//td");

    //        if (HTMLTableTRList != null && HTMLTableTRList.InnerText.Trim().Equals("NLA"))
    //        {
    //            return false;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Log the exception
    //        Console.WriteLine("Error in NLAGokanCheck: " + ex.Message);
    //        return false;
    //    }

    //    return true;
    //}


    public static async Task<bool> NLAGokanCheck(string mpn, IHttpClientFactory httpClientFactory, CancellationToken token = default(CancellationToken))
    {
        string htmlCode;

        try
        {
            using HttpClient client = httpClientFactory.CreateClient("NLOGokan");
            htmlCode = await client.GetStringAsync(new Uri("https://www.servicecasio.com/web/newpf/gokan.php?pcd=" + mpn + "&partscode=10557018&C1=1&C2=1&C3=1&C4=1&C5=1&C6=0&C7=0&C8=0&BRK=BRANK#"), token);
        }
        catch (Exception)
        {
            return false;
        }

        var InnerTable = htmlCode.Substring(htmlCode.IndexOf("<table border='1'"));
        var endek = InnerTable.IndexOf("/TABLE");
        InnerTable = InnerTable.Substring(0, endek + 7);
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(InnerTable);
        var HTMLTableTRList = from table in doc.DocumentNode.SelectNodes("//table").Cast<HtmlNode>()
            from row in table.SelectNodes("tr").Cast<HtmlNode>()
            from cell in row.SelectNodes("th|td").Cast<HtmlNode>()
            select new { Cell_Text = cell.InnerText };
        var ts = HTMLTableTRList.ElementAt(13).Cell_Text.ToString();
        if (ts.Trim().Equals("NLA"))
        {
            return false;
        }

        return true;
    }


   
}

public static class Xrates
{
       
    public static async Task<decimal> getXrate(DateTime date, string curr)
    {
        xrate kr = new xrate();
        using (var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext()))
        {
            var xrateservice = new EntityService<xrate>(unitOfWork);
            kr = await xrateservice.GetOneAsync(p => p.date.Year == date.Year && p.date.Month == date.Month && p.date.Day == date.Day && p.code.Equals(curr));

        }
        if (kr != null)
        {
            return Convert.ToDecimal(kr.rate);
        }
        var turl = "http://apilayer.net/api/historical?access_key=25da35b51c56962b9ba6f5f9c4e3ab05&date=" + date.ToString("yyyy-MM-dd"); ;
        string sonString = "";
        using (var client = new HttpClient())
        {
            var result = await client.GetAsync(turl);
            sonString = await result.Content.ReadAsStringAsync();
        }
        var i11 = sonString.IndexOf("GBP");
        var s11 = sonString.Substring(i11 + 5);
        var i21 = s11.IndexOf(",");
        var ix = "";
        ix = s11.Substring(0, i21);
        decimal jeden = Convert.ToDecimal(ix);

        var i12 = sonString.IndexOf(curr);
        var s12 = sonString.Substring(i12 + 5);
        var i22 = s12.IndexOf(",");
        var ixz = "";
        ixz = s12.Substring(0, i22);
        decimal dwa = Convert.ToDecimal(ixz);
        var kurs = (jeden / dwa);
        var xr = new xrate
        {
            date = date,
            rate = Convert.ToDecimal(jeden / dwa),
            code = curr,
            SourceCurrencyCode="GBP"


        };
        using (var unitOfWork = new UnitOfWork(DbContextFactory.GetWriteableContext()))
        {
            var xrateservice = new EntityService<xrate>(unitOfWork);
            await xrateservice.AddAsync(xr);
        }
        return kurs;
    }
}







   

public static class GetProductLog
{
    public static async Task<List<ItemEvent>> GetEventsFromStock(int itemBodyId,int locationId)
    {          
        var unitOfWork=new UnitOfWork(DbContextFactory.GetContext());
        var stockShotsService = new EntityService<stockshot>(unitOfWork);
        var itemEvents = (await stockShotsService.GetAllAsync(s => s.bodyid == itemBodyId && s.locationID == locationId)).Select(s => new ItemEvent
        {
            When = s.date,
            EventDescription = "Quantity at midnight: " + s.quantity.ToString()
        }).ToList();
            
        return itemEvents;
    }

    public static async Task<List<ItemEvent>> GetEventsFromOrders(int itemBodyId)
    {
        var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
        var OrderItemsService = new EntityService<orderitem>(unitOfWork);

        var itemEvents = (await OrderItemsService.GetAllIncludingAsync(
                s => s.itembodyID == itemBodyId,
                s => s.order)).Select(oi => new ItemEvent
            {
                When = oi.order.paidOn,
                EventDescription = oi.quantity + " products sold with order " + oi.orderID.ToString()
            })
            .ToList(); 
        return itemEvents;
    }

    public static async Task<List<ItemEvent>> GetEventsFromLog(int itemBodyID)
    {

        var unitOfWork = new UnitOfWork(DbContextFactory.GetContext());
        var logeventService = new EntityService<logevent>(unitOfWork);
        var itemEvents = (await logeventService.GetAllAsync(s => s.itemBodyID == itemBodyID)).Select(s => new ItemEvent
        {
            When = s.happenedOn,
            EventDescription = s._event
        }).ToList();
        return itemEvents;


    }
    public static async Task<List<ItemEvent>> GetLog(int itembodyID,int locationId)
    {
        var prodki = await GetEventsFromOrders(itembodyID);
          
        prodki.AddRange(await GetEventsFromStock(itembodyID, locationId));
        prodki.AddRange(await GetEventsFromLog(itembodyID));
        prodki = prodki.OrderByDescending(p => p.When).ToList();
        return prodki;
    }
}