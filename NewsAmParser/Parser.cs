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
        public int FailedArticleCount { get; set; }

        private string _baseAddress;

        public override IAsyncEnumerable<ResponseArticleModel> ParseAsync(string baseAdress, string requestUri)
        {
            _baseAddress = baseAdress;
            return new AsyncEnumerable<ResponseArticleModel>(async yield =>
            {
                try
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    var document = await LoadHtmlDocument(baseAdress, requestUri);
                    foreach (var baseArticle in LoadCurrentPageLinks(document))
                    {
                        try
                        {
                            var currentPage = await LoadHtmlDocument(baseArticle.BaseAddress, baseArticle.Address);
                            if (baseAdress != baseArticle.BaseAddress)
                            {
                                await yield.ReturnAsync(LoadSubDomainArticle(currentPage, baseArticle));
                            }
                            else
                            {
                                
                                await yield.ReturnAsync(LoadMainArticle(currentPage, baseArticle));
                            }
                            
                        }
                        catch (Exception e)
                        {
                            FailedArticleCount++;
                            Console.WriteLine(e);
                        }
                    }

                    watch.Stop();
                    Console.WriteLine(watch.Elapsed.Seconds);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        private async Task<HtmlDocument> LoadHtmlDocument(string baseAddress, string requestUri)
        {
            try
            {
                var document = new HtmlDocument();
                using (var restApi = new RestApiClient(baseAddress))
                {
                    restApi.SetCustomHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8")
                        .SetCustomHeader("Accept-Encoding", "gzip, deflate")
                        .SetCustomHeader("User-Agent",
                            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Safari/537.36")
                        .SetCustomHeader("accept-language", "en-US,en;q=0.9");
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
                var result = title.Attributes["href"].Value.GetRequestAndBaseAddress(_baseAddress);
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

        private ResponseArticleModel LoadMainArticle(HtmlDocument document, BaseArticleInfoModel baseArticle)
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


        private ResponseArticleModel LoadSubDomainArticle(HtmlDocument document, BaseArticleInfoModel baseArticle)
        {
            ResponseArticleModel article = new ResponseArticleModel
            {
                Title = baseArticle.Title,
                PublishedDateTimeUtc = baseArticle.PublishedDateTimeUtc,
                Content = GenerateContent(document, "div", "p", "opennewstext", false)
            };

            return article;
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
