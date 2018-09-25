using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Net;

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

        public static string GetXPathByClassName(this HtmlDocument document, string elementName, string className = null)
        {
            return $"//{elementName}{(string.IsNullOrEmpty(className) ? "" :  $"[@class = '{className}']")}";
        }

        public static string GetXPathByClassName(this HtmlDocument document, string elementName, string className, string childElement)
        {
            return $"//{elementName}{(string.IsNullOrEmpty(className) ? "" : $"[@class = '{className}']/{childElement}")}";
        }

        public static string GetXPathById(this HtmlDocument document, string elementName, string className = null)
        {
            return $"//{elementName}{(string.IsNullOrEmpty(className) ? "" :  $"[@id = '{className}']")}";
        }

        public static string GetInnerTextByClassName(this HtmlDocument document, string elementName, string className)
        {
            var result = document.DocumentNode
                                 .SelectSingleNode(document.GetXPathByClassName(elementName, className))
                                 .InnerText;

            return Regex.Replace(WebUtility.HtmlDecode(result), @"\s+", " ");
        }

        public static string GetAttribiuteValueByClassName(this HtmlDocument document, string elementName, string className, string attributeName)
        {
            var result = document.DocumentNode
                                 .SelectSingleNode(document.GetXPathByClassName(elementName, className))
                                 .Attributes[attributeName].Value;

            return WebUtility.HtmlDecode(result);
        }

        public static IEnumerable<HtmlNode> ElementWithoutChild(this HtmlNode node, string childElement)
        {
            foreach (HtmlNode childNode in node.ChildNodes)
            {
                if (childNode.Name == childElement && !childNode.HasChildNodes)
                    yield return childNode;
            }
        }

        public static string GetInnerTextById(this HtmlDocument document, string elementName, string id)
        {
            return document.DocumentNode
                .SelectSingleNode(document.GetXPathById(elementName, id))
                .InnerText;
        }

        public static string GetInnerTextByElementName(this HtmlNode htmlNode, string elementName)
        {
            var result = htmlNode.ChildNodes.FirstOrDefault(x => x.Name.Equals(elementName))?.InnerText;

            return WebUtility.HtmlDecode(result);
        }

        public static string GetAttribiuteValueById(this HtmlDocument document, string elementName, string id, string attributeName)
        {
            var result = document.DocumentNode
                                 .SelectSingleNode(document.GetXPathById(elementName, id))
                                 .Attributes[attributeName].Value;

            return WebUtility.HtmlDecode(result);
        }
    }
}
