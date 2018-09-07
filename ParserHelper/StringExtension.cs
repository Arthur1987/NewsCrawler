using System;
using System.Text.RegularExpressions;

namespace ParserHelper
{
    public static class StringExtension
    {
        public static (string, string) GetRequestAndBaseAddress(this string requestUri, string baseAddress)
        {
            if (Regex.IsMatch($"{Uri.UriSchemeHttps}:{requestUri}", @"^(http|https):\/\/|[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,6}(:[0-9]{1,5})?(\/.*)?$/ix"))
            {
                var correctUri = $"{Uri.UriSchemeHttps}:{requestUri}";

                if (!correctUri.Contains(Uri.SchemeDelimiter))
                {
                    correctUri = string.Concat(Uri.UriSchemeHttps, Uri.SchemeDelimiter, correctUri);
                }

                var uri = new Uri(correctUri);
                return (uri.AbsolutePath, new Uri(correctUri).Host);
            }
            return (requestUri, baseAddress);
        }
    }
}
