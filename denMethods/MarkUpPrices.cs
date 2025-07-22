using System.Globalization;

namespace denMethods;

public class MarkUpPrices
{

    public static string GetZeroPercentMarkupPrice(string currencySymbol, decimal pricePaid)
    {
        return currencySymbol + Math.Round((10 * (pricePaid + 2.5m)) / 7, 2).ToString();
    }
    public static string Get20PercentMarkupPrice(string currencySymbol, decimal pricePaid)
    {
        return currencySymbol + Math.Round(2 * (pricePaid + 2.5m), 2).ToString(CultureInfo.InvariantCulture);
    }
    public static string Get30PercentMarkupPrice(string currencySymbol, decimal pricePaid)
    {
        return currencySymbol + Math.Round((5 * (pricePaid + 2.5m)) / 2, 2).ToString();
    }
}