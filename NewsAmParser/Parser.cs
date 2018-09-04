using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ConstantDefine.Enums;
using HtmlAgilityPack;
using NewsAmParser.DataStructure;
using RestApi.Net.Core.Http;

namespace NewsAmParser
{
    public class Parser : BaseParser
    {
        public Parser(AppSetting appSetting) : base(appSetting)
        {
        }
        public override async Task<IEnumerable<ResponseArticleModel>> ParseAsync(NewsCategoryEnum category)
        {
            using (var restApi = new RestApiClient(AppSetting.NewsAm.BaseAddress))
            {
                restApi.SetCustomHeader("Accept", "text/html,application/xhtml+xml,application/xml");
                restApi.SetCustomHeader("Accept-Encoding", "gzip, deflate");
                restApi.SetCustomHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");
                restApi.SetCustomHeader("Accept-Charset", "ISO-8859-1");
                var content = await restApi.GetContentAsStringAsync(GetUri(category));
                var document = new HtmlDocument();
                document.Load(content);
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
    }
}
