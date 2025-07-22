using System.Text;

namespace denMethods;

public class Base64Converter
{
    public static string DecodeBase64ToString(string base64)
    {
        if (string.IsNullOrEmpty(base64))
        {
            return string.Empty;
        }

        try
        {
            // Spróbuj dekodować ciąg Base64.
            return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
        }
        catch (FormatException)
        {
            try
            {
                // Jeżeli nie powiedzie się, dodaj znaki '=' i spróbuj ponownie.
                base64 = AddPaddingToBase64(base64);
                return Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            }
            catch (FormatException)
            {
                // Jeżeli nadal nie powiedzie się, zwróć pusty ciąg.
                return string.Empty;
            }
        }
    }

    private static string AddPaddingToBase64(string base64)
    {
        int mod = base64.Length % 4;
        return mod > 0 ? base64.PadRight(base64.Length + (4 - mod), '=') : base64;
    }
}