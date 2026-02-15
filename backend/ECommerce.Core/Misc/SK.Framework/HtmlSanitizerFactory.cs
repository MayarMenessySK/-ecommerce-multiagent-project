using Ganss.Xss;
using Markdig;


namespace SK.Framework;

public static class HtmlSanitizerFactory
{
    static HtmlSanitizer _default = default!;

    public static HtmlSanitizer Default
    {
        get
        {
            if (_default is null)
                _default = new HtmlSanitizer();

            return _default;
        }
    }


    static HtmlSanitizer _allowCss = default!;
    public static HtmlSanitizer AllowCss
    {
        get
        {
            if (_allowCss == null)
            {
                _allowCss = new HtmlSanitizer();
                _allowCss.AllowedAttributes.Add("class");
            }

            return _allowCss;
        }
    }

    static HtmlSanitizer _allowMailTo = default!;
    public static HtmlSanitizer AllowMailTo
    {
        get
        {
            if (_allowMailTo == null)
            {
                _allowMailTo = new HtmlSanitizer();
                _allowMailTo.AllowedSchemes.Add("mailto");
            }

            return _allowMailTo;
        }
    }

    static HtmlSanitizer _allowMailToAndCss = default!;
    public static HtmlSanitizer AllowMailToAndCss
    {
        get
        {
            if (_allowMailToAndCss == null)
            {
                _allowMailToAndCss = new HtmlSanitizer();
                _allowMailToAndCss.AllowedAttributes.Add("class");
                _allowMailToAndCss.AllowedSchemes.Add("mailto");
            }

            return _allowMailToAndCss;
        }
    }

    public static string Sanitize2(this HtmlSanitizer sanitizer, string? val)
    {
        var result = RemoveMaliciousPatterns(val);

        return sanitizer.Sanitize(result);
    }

    public static List<string> SanitizeList(this HtmlSanitizer sanitizer, List<string>? val)
    {
        if (val == null)
            return new List<string>();

        var s = HtmlSanitizerFactory.Default;

        return val.Select(x => s.Sanitize2(x)).ToList();
    }

    private static readonly List<string> DangerousStartsWithPatterns = new List<string>
    {
        // Formula starters
        "=", "+", "-", "@",
    
        // Obfuscation attempts (when at start)
        "CHAR(61)",  // Unicode for '='
        "\t",        // Tab
        "\r",        // Carriage return
        "\n"         // Newline
    };
    private static readonly List<string> DangerousContainsPatterns = new List<string>
    {
        // Command execution
        "cmd|", "' /C", "' /K", " /C", " /K",
        "|",    // Pipe character (often used in attacks)
    
        // Remote data calls
        "WEBSERVICE(", "IMPORTXML(", "IMPORTHTML(", "FILTERXML(",
    
        // Hyperlink exploits
        "HYPERLINK(",
    
        // DDE/OLE attacks
        "DDE(", "EMBED(",
    
        // Excel 4.0 macros
        "EXEC(", "RUN(", "CALL(",
    
        // Dynamic cell reference
        "INDIRECT("
    };
    public static string RemoveMaliciousPatterns(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        string trimmedInput = input.Trim();

        // 1. Check for dangerous patterns at the START of the cell
        foreach (var pattern in DangerousStartsWithPatterns)
        {
            if (trimmedInput.StartsWith(pattern, StringComparison.OrdinalIgnoreCase))
            {
                // Escape formula starters with a single quote
                return "'" + input;
            }
        }

        // 2. Check for dangerous patterns ANYWHERE in the cell
        foreach (var pattern in DangerousContainsPatterns)
        {
            if (trimmedInput.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            {
                // Block outright malicious payloads
                return "[BLOCKED - SECURITY RISK]";
            }
        }

        return input;
    }
    public static class MarkdownPipelineHelper
    {
        public static MarkdownPipeline MarkdownPipelines
        {
            get
            {
                var pipeline = new Markdig.MarkdownPipelineBuilder();
                pipeline.UseAdvancedExtensions().UseSoftlineBreakAsHardlineBreak();
                return pipeline.Build();
            }
        }

        public static string? RenderSafeHtml(string? markdown)
        {
            if (string.IsNullOrWhiteSpace(markdown))
                return markdown;
            var html = Markdown.ToHtml(markdown, MarkdownPipelines);
            return HtmlSanitizerFactory.Default.Sanitize2(html);
        }
    }
}