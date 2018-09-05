using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace ParserHelper.HtmlDocumentExtension
{
    public static class HtmlAgilityPackExtension
    {
        public static IEnumerable<HtmlNode> GetElementsWithClass(this HtmlDocument docucment, string elemenName, string className)
        {

            Regex regex = new Regex("\\b" + Regex.Escape(className) + "\\b", RegexOptions.Compiled);

            var result = docucment
                .DocumentNode
                .ChildNodes
                .Where(n => n.NodeType == HtmlNodeType.Element)
                .Where(e => e.Name == elemenName && regex.IsMatch(e.GetAttributeValue("class", "")));

            return result;
        }

        public static string GetXPath(this HtmlDocument docucment, string elementName, string className)
        {
            return $"//{elementName}[@class = '{className}']";
        }
    }
}
