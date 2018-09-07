using ConstantDefine.Enums;
using HtmlAgilityPack;
using NewsAmParser.DataStructure;
using RestApi.Net.Core.Http;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ParserHelper;
using ParserHelper.HtmlDocumentExtension;

namespace NewsAmParser
{
    public class Parser : BaseParser
    {
        public Parser(AppSetting appSetting) : base(appSetting)
        {
        }
        public override IAsyncEnumerable<ResponseArticleModel> ParseAsync(NewsCategoryEnum category)
        {
            return new AsyncEnumerable<ResponseArticleModel>(async yield =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                var result = GetUri(category).GetRequestAndBaseAddress(AppSetting.NewsAm.BaseAddress);
                var document = await LoadHtmlLocument(result.Item2, result.Item1);
                foreach (var link in LoadCurrentPageLinks(document))
                {
                    result = link.GetRequestAndBaseAddress(AppSetting.NewsAm.BaseAddress);
                    var currentPage = await LoadHtmlLocument(result.Item2,result.Item1);
                    await yield.ReturnAsync(LoadArticle(currentPage));
                }

                watch.Stop();
                Console.WriteLine(watch.Elapsed.Seconds);
            });
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
            try
            {
                var document = new HtmlDocument();
                using (var restApi = new RestApiClient(baseAddress))
                {
                    restApi.SetCustomHeader("Accept", "text/html,application/xhtml+xml,application/xml")
                        .SetCustomHeader("Accept-Encoding", "gzip, deflate")
                        .SetCustomHeader("User-Agent",
                            "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0")
                        .SetCustomHeader("Accept-Charset", "ISO-8859-1");
                    var content = await restApi.GetContentAsStringAsync(requestUri);
                    document.LoadHtml(content);
                }

                return document;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        

        private IEnumerable<string> LoadCurrentPageLinks(HtmlDocument document)
        {
            IEnumerable<string> urls = document.DocumentNode
                                               .SelectNodes(document.GetXPath("a", "photo-link"))
                                               .Select(x => x.Attributes["href"].Value);
            return urls;
        }

        private ResponseArticleModel LoadArticle(HtmlDocument document)
        {
            ResponseArticleModel article = new ResponseArticleModel
            {
                Title = document.GetInnerTextByClassName("div", "article-title"),
                PublishedDateTime = DateTimeOffset.ParseExact(document.GetAttribiuteValueByClassName("div", "time", "content"), "yyyy-MM-ddzHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture).UtcDateTime,
                Content = GenerateContent(document)
            };

            return article;
        }

        public string GenerateContent(HtmlDocument document)
        {
            var paragraphs = document.DocumentNode
                .SelectSingleNode(document.GetXPath("span", "article-body"))
                .Elements("p");

            return string.Join(" ", paragraphs.Select(x => x.InnerText)) ;
        }

    }
}
