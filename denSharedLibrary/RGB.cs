namespace denSharedLibrary;

public static class Colours
{
    public class RGB {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
    }

    private static List<string> hexColors = new List<string>
    {
        "#ffff00","#0000ff", "#ff0000", "#2f4f4f", "#8b4513", "#006400", "#bdb76b", "#00008b",
        "#00ced1", "#00ced1","#911eb4","#bfef45","#f58231","#ffd8b1","#fabed4"
    };

    public static List<RGB> GetBaseColors(int number)
    {
        var rgbList = new List<RGB>();
        Random random = new Random();

        // Convert hex colors to RGB
        foreach (var hexColor in hexColors)
        {
            var rgb = new RGB
            {
                R = Convert.ToByte(hexColor.Substring(1, 2), 16),
                G = Convert.ToByte(hexColor.Substring(3, 2), 16),
                B = Convert.ToByte(hexColor.Substring(5, 2), 16)
            };
            rgbList.Add(rgb);
        }

        // If the requested number of colors is greater than the base colors, generate random colors
        while (rgbList.Count < number)
        {
            var newColor = new RGB
            {
                R = (byte)random.Next(256),
                G = (byte)random.Next(256),
                B = (byte)random.Next(256)
            };

            bool isSimilar = false;
            foreach (var existingColor in rgbList)
            {
                int colorDifference = Math.Abs(existingColor.R - newColor.R) +
                                      Math.Abs(existingColor.G - newColor.G) +
                                      Math.Abs(existingColor.B - newColor.B);

                if (colorDifference < 50)  // Adjust threshold as needed
                {
                    isSimilar = true;
                    break;
                }
            }

            if (!isSimilar)
            {
                rgbList.Add(newColor);
            }
        }

        return rgbList;
    }


}