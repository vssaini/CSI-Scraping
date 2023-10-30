namespace CSI.WebScraping.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Remove dollar sign and convert to decimal.
    /// </summary>
    public static decimal ToDecimal(this string value)
    {
        value = value.Replace("$", string.Empty);
        return decimal.TryParse(value, out var result) ? result : 0;
    }

    public static int ToInt(this string value)
    {
        return int.TryParse(value, out var result) ? result : 0;
    }
}