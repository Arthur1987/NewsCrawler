using System;
using System.Threading.Tasks;
using ConstantDefine.Enums;
using NewsAmParser;
using NewsAmParser.DataStructure;
using NewsAmParser = NewsAmParser.Parser;

namespace NewsCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppSetting app = new AppSetting
            {
                NewsAm = new NewsAm
                {
                    ArmenianNews = "eng/news/armenia/allthemes/2018/09/01/",
                    BaseAddress = "https://news.am/"
                }
            };
            Parser parser  = new Parser(app);
            await parser.ParseAsync(NewsCategoryEnum.ArmenianDiaspora);
            //string basePath = System.AppContext.BaseDirectory;
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //    .SetBasePath(basePath)
            //    .AddJsonFile("config.json")
            //    .Build();
            Console.WriteLine("Hello World!");
        }
    }
}
