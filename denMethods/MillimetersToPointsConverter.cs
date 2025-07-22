namespace denMethods;

public static class MillimetersToPointsConverter
{
    private const float MillimetersPerInch = 25.4f;
    private static float PointsPerInch = 72f;

    public static void SetDPI(float dpi)
    {
        PointsPerInch = dpi;
    }

    public static float Convert(float millimeters)
    {
        // Przelicz milimetry na cale
        float inches = millimeters / MillimetersPerInch;

        // Przelicz cale na punkty
        float points = inches * PointsPerInch;
        return points;
    }
}