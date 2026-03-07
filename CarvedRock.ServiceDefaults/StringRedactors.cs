using System.Text.RegularExpressions;

namespace CarvedRock.ServiceDefaults;

public static partial class StringRedactors
{
    public static string MaskSsn(this string input)
    {
        return input.MaskFirstNDigits(6);
    }
    public static string MaskFirstNDigits(this string input, int digitsToMask, char maskChar = '*')
    {
        int seen = 0;
        return Digit().Replace(input, m =>
            (++seen <= digitsToMask) ? maskChar.ToString() : m.Value);
    }

    [GeneratedRegex(@"\d")]
    private static partial Regex Digit();
}