using ConstantDefine.Enums;
using HtmlAgilityPack;
using NewsAmParser.DataStructure;
using RestApi.Net.Core.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ParserHelper.HtmlDocumentExtension;

namespace NewsAmParser
{
    public class Parser : BaseParser
    {
        public Parser(AppSetting appSetting) : base(appSetting)
        {
        }
        public override async Task<IEnumerable<ResponseArticleModel>> ParseAsync(NewsCategoryEnum category)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                var document = await LoadHtmlLocument(AppSetting.NewsAm.BaseAddress, GetUri(category));
                foreach (var link in LoadCurrentPageLinks(document))
                {
                    var currentPage = await LoadHtmlLocument(AppSetting.NewsAm.BaseAddress, link);
                }
                watch.Stop();
                Console.WriteLine(watch.Elapsed.Seconds);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return null;

        }

        private string GetUri(NewsCategoryEnum category)
        {
            switch (category)
            {
                case NewsCategoryEnum.ArmenianDiaspora:
                    return AppSetting.NewsAm.ArmenianNews;
                default:
                    throw new NotSupportedException();
            }
        }

        private async Task<HtmlDocument> LoadHtmlLocument(string baseAddress, string requestUri)
        {
            var document = new HtmlDocument();
            using (var restApi = new RestApiClient(AppSetting.NewsAm.BaseAddress))
            {
                restApi.SetCustomHeader("Accept", "text/html,application/xhtml+xml,application/xml")
                    .SetCustomHeader("Accept-Encoding", "gzip, deflate")
                    .SetCustomHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0")
                    .SetCustomHeader("Accept-Charset", "ISO-8859-1");
                var content = await restApi.GetContentAsStringAsync(requestUri);
                document.LoadHtml(content);
            }

            return document;
        }


        private IEnumerable<string> LoadCurrentPageLinks(HtmlDocument document)
        {

            var articleList = document.GetElementsWithClass("div", "articles-list");

            IEnumerable<string> urls = document.DocumentNode
                                               .SelectNodes(document.GetXPath("a", "photo-link"))
                                               .Select(x => x.Attributes["href"].Value);
            //article - item
            return urls;
        }

    }
}
