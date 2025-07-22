using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace denSharedLibrary;

public static class MillimetersToPointsConverter
{
    private const float MillimetersPerInch = 25.4f;
    private const float PointsPerInch = 72f;

    public static float Convert(float millimeters)
    {
        // Przelicz milimetry na cale
        float inches = millimeters / MillimetersPerInch;

        // Przelicz cale na punkty
        float points = inches * PointsPerInch;

        return points;
    }
}
public class BoxLabelModel
{
    public string TopSmallText { get; set; }
    public string CentralLargeText { get; set; }
    public string CentralSmallText { get; set; }
    public string Subtitle { get; set; }
    public float Width { get; set; }
    public float Height { get; set; }
    public float LargeFont { get; set; }
    public float LesserFont { get; set; }
    public float TopFont { get; set; }
    public float BottomFont { get; set; }


}

public class BoxLabelToImageByteArray : IBoxLabelToImageByteArray
{
    private class SummaryDocument : IDocument
    {
        public BoxLabelModel Model { get; }

        public SummaryDocument(BoxLabelModel model)
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
                    page.MarginVertical(4);
                    page.MarginHorizontal(6);
                    page.DefaultTextStyle(x => x.FontFamily("Calibri"));
                    float width = MillimetersToPointsConverter.Convert(Model.Width);
                    float height = MillimetersToPointsConverter.Convert(Model.Height);
                    page.Size(new PageSize(width, height));
                    page.Header().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);
                    page.Footer().Element(ComposeFooter);
                });
        }

        void ComposeFooter(QuestPDF.Infrastructure.IContainer container)
        {
            container.AlignCenter().Column
            (
                column =>
                {
                    column.Item().AlignBottom().Text(Model.Subtitle).FontSize(Model.BottomFont);
                }
            );
        }

        void ComposeHeader(QuestPDF.Infrastructure.IContainer container)
        {
            container.AlignCenter().Column
            (
                column =>
                {
                    column.Item().AlignBottom().Text(Model.TopSmallText).FontSize(Model.TopFont);
                }
            );
        }

        void ComposeContent(QuestPDF.Infrastructure.IContainer container)
        {
            container.AlignCenter().AlignMiddle().Column
            (
                column =>
                {
                    column.Item().AlignCenter().Text(Model.CentralLargeText).FontSize(Model.LargeFont).Bold().LineHeight(0.8f);
                    column.Item().AlignCenter().Text(Model.CentralSmallText).FontSize(Model.LesserFont).Black().LineHeight(0.8f);
                }
            );
        }
    }

    private BoxLabelModel GetBoxLabelModel(string topSmallText, string centralLargeText, string centralSmallText, string subtitle, float width, float height, float largeFont, float lesserFont, float topFont, float bottomFont)
    {
        return new BoxLabelModel
        {
            Subtitle = subtitle,
            CentralLargeText = centralLargeText,
            CentralSmallText = centralSmallText,
            TopSmallText = topSmallText,
            Height = height,
            Width = width,
            LargeFont = largeFont,
            LesserFont = lesserFont,
            TopFont = topFont,
            BottomFont = bottomFont

        };
    }

    public byte[] GenerateImage(string topSmallText, string centralLargeText, string centralSmallText, string subtitle, float width, float height, float largeFont, float lesserFont, float topFont, float bottomFont)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var model = GetBoxLabelModel(topSmallText, centralLargeText, centralSmallText, subtitle, width, height, largeFont, lesserFont, topFont, bottomFont);
        IDocument document = new SummaryDocument(model);
        return document.GenerateImages().First();
    }

    public byte[] GenerateImage(LabelNamePack LabelNamePack, LabelProperties LabelProperty)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var model = GetBoxLabelModel(LabelNamePack.Toptext, LabelNamePack.CentralLargeText, LabelNamePack.CentralSmallText, LabelNamePack.Bottomtext,
            LabelProperty.Width, LabelProperty.Height,(float) LabelProperty.LargeFont, (float)LabelProperty.LesserFont, (float)LabelProperty.TopFont, (float)LabelProperty.BottomFont);
        IDocument document = new SummaryDocument(model);
        return document.GenerateImages().First();
    }

}