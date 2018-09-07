using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConstantDefine.Enums;
using NewsAmParser;
using NewsAmParser.DataStructure;

namespace NewsCrawler
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var currentDate = DateTime.Now.AddDays(-1);
            AppSetting app = new AppSetting
            {
                NewsAm = new NewsAm
                {
                    ArmenianNews = $"eng/news/allregions/allthemes/{currentDate.Year}/{currentDate.Month}/{currentDate.Day}",
                    BaseAddress = "https://news.am/"
                }
            };
            Parser parser  = new Parser(app);
            var result  =  parser.ParseAsync(NewsCategoryEnum.ArmenianDiaspora);
            var listOfItemns = new List<ResponseArticleModel>();
            await result.ForEachAsync(item => {
                listOfItemns.Add(item);
            });
            Console.WriteLine($"Total articles count : {listOfItemns.Count}");
            //string basePath = System.AppContext.BaseDirectory;
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //    .SetBasePath(basePath)
            //    .AddJsonFile("config.json")
            //    .Build();
            Console.WriteLine("Hello World!");
        }
    }
}
