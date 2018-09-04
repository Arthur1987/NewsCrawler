using System;
using ConstantDefine.Enums;
using NewsAmParser;
using NewsAmParser.DataStructure;
using NewsAmParser = NewsAmParser.Parser;

namespace NewsCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            AppSetting app = new AppSetting
            {
                NewsAm = new NewsAm
                {
                    ArmenianNews = "armenia/allthemes/2018/09/01/",
                    BaseAddress = "https://news.am/eng/news/"
                }
            };
            Parser parser  = new Parser(app);
            parser.ParseAsync(NewsCategoryEnum.ArmenianDiaspora).GetAwaiter().GetResult();
            //string basePath = System.AppContext.BaseDirectory;
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //    .SetBasePath(basePath)
            //    .AddJsonFile("config.json")
            //    .Build();
            Console.WriteLine("Hello World!");
        }
    }
}
