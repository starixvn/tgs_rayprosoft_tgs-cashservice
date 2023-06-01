namespace Tgs.CashService;

internal static class DecimalExtensions
{
    public static decimal Truncate(this decimal d, int digits)
    {
        var num = (decimal)Math.Pow(10.0, digits);
        return decimal.Truncate(d * num) / num;
    }
}
