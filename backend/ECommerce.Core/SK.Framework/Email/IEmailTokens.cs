using MimeKit;

namespace SK.Framework.Email;

public interface IEmailTokens
{
    string Lang { get; set; }
}

public interface IDbEmailSingleSender<T> where T : IEmailTokens
{
    Dictionary<SupportedLanguage, string> Templates { get; }

    SupportedLanguage Lang { get; set; }

    T Tokens { get; set; }

    string? LayoutFile { get; set; }

    MailboxAddress? To { get; set; }
}

public interface IFileEmailSingleSender<T> where T : IEmailTokens
{
    string TemplateFolder { get; }

    Dictionary<SupportedLanguage, string> Templates { get; }

    SupportedLanguage Lang { get; set; }

    T Tokens { get; set; }

    string? LayoutFile { get; set; }

    MailboxAddress? To { get; set; }
}

public interface IDataEmailSingleSender<T> where T : IEmailTokens
{
    public string SubjectTemplate { get; set; }

    public string BodyTemplate { get; set; }

    SupportedLanguage Lang { get; set; }

    T Tokens { get; set; }

    string? LayoutFile { get; set; }

    MailboxAddress? To { get; set; }
}