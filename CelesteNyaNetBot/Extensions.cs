namespace CelesteNyaNetBot;

public static class Extensions
{
    public static bool EqualsIgnoreCases(this string thisString, string str)
        => thisString.Equals(str, StringComparison.InvariantCultureIgnoreCase);
}
