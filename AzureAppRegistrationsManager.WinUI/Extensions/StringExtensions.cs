namespace AzureAppRegistrationsManager.WinUI.Extensions;

internal static class StringExtensions
{
    internal static List<string> SplitToList(this string input)
    {
        return input
            .Split(['\n', '\r'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Distinct()
            .ToList();
    }

    internal static string JoinToString(this IEnumerable<string>? items)
    {
        if (items == null || !items.Any())
        {
            return string.Empty;
        }

        return string.Join("\n", items.OrderBy(u => u));
    }
}