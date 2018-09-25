using ConstantDefine.Enums;
using HtmlAgilityPack;
using NewsAmParser.DataStructure;
using ParserHelper;
using ParserHelper.HtmlDocumentExtension;
using RestApi.Net.Core.Http;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

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
                try
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    var result = GetUri(category).GetRequestAndBaseAddress(AppSetting.NewsAm.BaseAddress);
                    var document = await LoadHtmlDocument(result.Item2, result.Item1);
                    foreach (var baseArticle in LoadCurrentPageLinks(document))
                    {
                        try
                        {
                            var currentPage = await LoadHtmlDocument(baseArticle.BaseAddress, baseArticle.Address);
                            await yield.ReturnAsync(LoadArticle(currentPage, AppSetting.NewsAm.BaseAddress != baseArticle.BaseAddress, baseArticle));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }

                    watch.Stop();
                    Console.WriteLine(watch.Elapsed.Seconds);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
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

        private async Task<HtmlDocument> LoadHtmlDocument(string baseAddress, string requestUri)
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
                        .SetCustomHeader("Accept-Charset", "utf-8");
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
        

        private IEnumerable<BaseArticleInfoModel> LoadCurrentPageLinks(HtmlDocument document)
        {
            var articleNodes = document.DocumentNode
                                       .SelectSingleNode("//div[contains(@class, 'articles-list') and contains(@class, 'casual')]")
                                       .ChildNodes
                                       .Where(x => x.Name == "article");
            foreach (var item in articleNodes)
            {
                var title = item.SelectSingleNode(".//div[@class='title']/a");
                var result = title.Attributes["href"].Value.GetRequestAndBaseAddress(AppSetting.NewsAm.BaseAddress);
                if (AppSetting.NewsAm.BaseAddress == result.Item2)
                {
                    BaseArticleInfoModel article = new BaseArticleInfoModel
                    {
                        Title = title.InnerText,
                        BaseAddress = result.Item2,
                        Address = result.Item1,
                        PublishedDateTimeUtc =
                            DateTimeOffset
                                .ParseExact(
                                    item.SelectSingleNode(".//div[@class='date']/time").Attributes["datetime"].Value,
                                    "yyyy-MM-ddzHH:mm:ss", CultureInfo.InvariantCulture).UtcDateTime
                    };
                    yield return article;
                }
            }
                    
        }

        private ResponseArticleModel LoadArticle(HtmlDocument document, bool isSubdomain, BaseArticleInfoModel baseArticle)
        {
            return isSubdomain ? LoadSubDomainArticle(document, CultureInfo.InvariantCulture, baseArticle) : LoadMainArticle(document, baseArticle); //CultureInfo.CreateSpecificCulture("HY") "dd MMMM, yyyy  HH:mm",
        }

        private ResponseArticleModel LoadMainArticle(HtmlDocument document, BaseArticleInfoModel baseArticle)
        {
            try
            {

                ResponseArticleModel article = new ResponseArticleModel
                {
                    Title = baseArticle.Title,
                    PublishedDateTimeUtc = baseArticle.PublishedDateTimeUtc,
                    ParseDateTime = DateTime.UtcNow,
                    Content = GenerateContent(document, "span", "p", "article-body")
                };

                return article;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private ResponseArticleModel LoadSubDomainArticle(HtmlDocument document, CultureInfo cultureInfo, BaseArticleInfoModel baseArticle)
        {
            try
            {
                ResponseArticleModel article = new ResponseArticleModel
                {
                    Title = document.DocumentNode.SelectSingleNode(document.GetXPathById("div", "opennews")).SelectSingleNode("//h1").InnerText,
                        //.GetInnerTextByElementName("h1"),
                    PublishedDateTimeUtc = TryToGetPublisheDateTime(document, cultureInfo),
                    Content = GenerateContent(document, "div", "p", "opennewstext", false)
                };

                return article;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private DateTime TryToGetPublisheDateTime(HtmlDocument document, CultureInfo cultureInfo)
        {
            var timeDiv = document.DocumentNode.SelectSingleNode(document.GetXPathByClassName("div", "time"));
            if (timeDiv != null)
            {
                return DateTimeOffset.ParseExact(document.GetInnerTextByClassName("div", "time"), "MMMM d, yyyy HH:mm", cultureInfo)
                                     .UtcDateTime;
            }
            else
            {
                string timeComponent = document.GetInnerTextByClassName("span", "time").Trim();
                string dateComponent = document.GetInnerTextByClassName("span", "date").Trim();
                string datetimeText = $"{dateComponent} {timeComponent}".Trim();
                
                return DateTimeOffset.ParseExact(datetimeText, "z, MMMM d, yyyy HH:mm", cultureInfo).UtcDateTime;

            }
        }

        public string GenerateContent(HtmlDocument document, string parentElement,  string childElement, string parentElementAttribute, bool isClass = true)
        {
            var paragraphs = document.DocumentNode
                                     .SelectSingleNode(isClass ?  document.GetXPathByClassName(parentElement, parentElementAttribute) : document.GetXPathById(parentElement, parentElementAttribute))
                                     .Elements(childElement);

            return string.Join(" ", paragraphs.Select(x => x.InnerText)) ;
        }
    }
}
