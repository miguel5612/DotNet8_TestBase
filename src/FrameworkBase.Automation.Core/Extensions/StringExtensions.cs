using System.Text.RegularExpressions;

namespace FrameworkBase.Automation.Core.Extensions;

public static partial class StringExtensions
{
    [GeneratedRegex(@"\s+")]
    private static partial Regex WhiteSpaceRegex();

    public static string NormalizeWhitespace(this string value)
    {
        return WhiteSpaceRegex().Replace(value.Trim(), " ");
    }
}
