using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ParserHelper.HtmlDocumentExtension
{
    public static class HtmlAgilityPackExtension
    {
        public static IEnumerable<HtmlNode> GetElementsWithClass(this HtmlDocument document, string elemenName, string className)
        {

            Regex regex = new Regex("\\b" + Regex.Escape(className) + "\\b", RegexOptions.Compiled);

            var result = document
                .DocumentNode
                .ChildNodes
                .Where(n => n.NodeType == HtmlNodeType.Element)
                .Where(e => e.Name == elemenName && regex.IsMatch(e.GetAttributeValue("class", "")));

            return result;
        }

        public static string GetXPath(this HtmlDocument document, string elementName, string className = null)
        {
            return $"//{elementName}{(string.IsNullOrEmpty(className) ? "" :  $"[@class = '{className}']")}";
        }

        public static string GetInnerTextByClassName(this HtmlDocument document, string elementName, string className)
        {
            return document.DocumentNode
                           .SelectSingleNode(document.GetXPath(elementName, className))
                           .InnerText;
        }

        public static string GetAttribiuteValueByClassName(this HtmlDocument document, string elementName, string className, string attributeName)
        {
            return document.DocumentNode
                .SelectSingleNode(document.GetXPath(elementName, className))
                .Attributes[attributeName].Value;
        }
    }
}
