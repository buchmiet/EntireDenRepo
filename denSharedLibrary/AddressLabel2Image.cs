using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace denSharedLibrary;

public class AddressModel
{
    public string[] Lines { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
public class AddressLabel2Image
{

    public class SummaryDocument : IDocument
    {
        public AddressModel Model;

        public SummaryDocument(AddressModel model)
        {
            Model = model;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.MarginVertical(2, Unit.Millimetre);
                    page.MarginHorizontal(2, Unit.Millimetre);
                    page.DefaultTextStyle(x => x.FontFamily("Calibri"));
                    page.Size(new PageSize(Model.Width, Model.Height, Unit.Millimetre));
                    page.Content().AlignMiddle().AlignCenter().ScaleToFit().Element(ComposeContent);
                });
        }

        void ComposeContent(QuestPDF.Infrastructure.IContainer container)
        {
            var textsize = MillimetersToPointsConverter.Convert((Model.Height - 15)) / Model.Lines.Count();
            container.Column(column =>
            {
                for (int i = 0; i < Model.Lines.Count(); i++)
                {
                    column.Item().Text(Model.Lines[i]).FontSize(textsize);
                }

            });
        }
    }
    public byte[] GenerateImages(string adres, int width, int height)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        IDocument document = new SummaryDocument(new AddressModel { Lines = adres.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None), Width = width, Height = height });
        MemoryStream myStream = new();
        document.GenerateXps(@"c:\buchmiet.ltd\addresslabel3.xps");
        return document.GenerateImages().First();
    }
}