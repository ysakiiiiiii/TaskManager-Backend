using System.Text.RegularExpressions;
using Ganss.Xss;

namespace TaskManagerBackend.Helpers
{
    public static class InputSanitizer
    {
        private static readonly HtmlSanitizer _htmlSanitizer = new HtmlSanitizer();
        private static readonly Regex _sqlInjectionRegex = new Regex(
            @"(--|;|'|""|/\*|\*/|xp_|exec\s|union\s|select\s|insert\s|delete\s|update\s|drop\s|alter\s)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled,
            TimeSpan.FromMilliseconds(100)
        );

        public static string Sanitize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            string sanitized = input.Trim();

            sanitized = SanitizeSql(sanitized);

            sanitized = SanitizeHtml(sanitized);

            return sanitized;
        }

        private static string SanitizeSql(string input)
        {
            return _sqlInjectionRegex.Replace(input, string.Empty);
        }

        private static string SanitizeHtml(string input)
        {
            try
            {
                return _htmlSanitizer.Sanitize(input);
            }
            catch
            {
                return Regex.Replace(input, "<.*?>", string.Empty);
            }
        }

        public static string SanitizeUsername(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return Regex.Replace(input.Trim(), @"[^\w\-@\.]", string.Empty);
        }

        public static string SanitizeEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return Regex.Replace(input.Trim(), @"[^\w\-@\.\+]", string.Empty);
        }
    }
}