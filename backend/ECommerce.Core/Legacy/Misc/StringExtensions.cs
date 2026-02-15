using System.Text;
using System.Text.RegularExpressions;

namespace ECommerce.Core.Misc;

public static class StringExtensions
{
    public static string ToSlug(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        // Convert to lowercase
        text = text.ToLowerInvariant();

        // Remove accents
        text = RemoveAccents(text);

        // Replace spaces with hyphens
        text = Regex.Replace(text, @"\s+", "-");

        // Remove invalid characters
        text = Regex.Replace(text, @"[^a-z0-9\-]", "");

        // Remove duplicate hyphens
        text = Regex.Replace(text, @"-+", "-");

        // Trim hyphens from start and end
        text = text.Trim('-');

        return text;
    }

    public static string Truncate(this string text, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            return text;

        return text.Substring(0, maxLength - suffix.Length) + suffix;
    }

    public static string RemoveAccents(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return text;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public static bool IsValidEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
        catch
        {
            return false;
        }
    }

    public static string? NullIfEmpty(this string? text)
    {
        return string.IsNullOrWhiteSpace(text) ? null : text;
    }
}
