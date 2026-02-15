using System;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace SK.Framework.MVC;

public static class Fmt2
{
    public static IHtmlContent Mode(FormMode mode)
        => (mode == FormMode.Edit) ? new HtmlString("Update") : new HtmlString("Create");

    public static IHtmlContent Email(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return new HtmlString("");

        string link = $"<a href=\"mailto:{email}\">{email}</a>";
        return new HtmlString(link);
    }

    public static IHtmlContent Link(string url, bool isRelative = false, bool isNewWindow = false)
    {
        if (string.IsNullOrWhiteSpace(url))
            return new HtmlString("");

        string newWindowAttr = "";

        if (isNewWindow)
            newWindowAttr = " target=\"new\" ";

        if (isRelative || url.Contains("http"))
        {
            string link = $"<a href=\"{url}\"{newWindowAttr}>{url}</a>";
            return new HtmlString(link);
        }

        string link2 = $"<a href=\"http://{url}\"{newWindowAttr}>{url}</a>";
        return new HtmlString(link2);
    }

    public static IHtmlContent Date(DateTime? dateTime) =>
       (!dateTime.HasValue) ? new HtmlString("") : new HtmlString(dateTime.Value.ToString("D"));

    public static IHtmlContent ArabicNumber(double? number)
    {
        if (!number.HasValue)
            return new HtmlString("");

        if (Env.IsArabic())
            return new HtmlString(ConvertToEasternArabicNumerals(number.HasValue ? number.Value.ToString("n") : number.ToString()));

        return new HtmlString($"{number}");
    }

    public static string ConvertToEasternArabicNumerals(string input)
    {
        var utf8Encoder = new UTF8Encoding();
        var utf8Decoder = utf8Encoder.GetDecoder();
        var convertedChars = new StringBuilder();
        char[] convertedChar = new char[1];
        byte[] bytes = new byte[] { 217, 160 };
        char[] inputCharArray = input.ToCharArray();
        foreach (char c in inputCharArray)
        {
            if (char.IsDigit(c))
            {
                bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
                utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
                convertedChars.Append(convertedChar[0]);
            }
            else
            {
                convertedChars.Append(c);
            }
        }

        return convertedChars.ToString();
    }
}
