using System;

namespace SK.Framework;

public enum SupportedLanguage
{
    None,
    Arabic,
    English
}

public static class SupportedLanguageExtensions
{
    public static string Code(this SupportedLanguage self)
    {
        switch (self)
        {
            case SupportedLanguage.Arabic:
                return "ar";

            case SupportedLanguage.English:
                return "en";

            default: throw new ArgumentOutOfRangeException();
        }
    }

    public static SupportedLanguage ToSupportedLang(this string self)
    {
        switch (self)
        {
            case "ar": return SupportedLanguage.Arabic;
            case "en": return SupportedLanguage.English;
            default: throw new ArgumentOutOfRangeException();
        }
    }
}
