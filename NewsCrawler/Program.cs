using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Diagnostics;
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
            //HtmlWeb

            Stopwatch watch = new Stopwatch();
            watch.Start();
            var currentDate = DateTime.Now;
            int year = DateTime.Now.Year;
            DateTime firstDay = new DateTime(year, 1, 1);
            var listOfItemns = new List<ResponseArticleModel>();
            while (currentDate >= firstDay)
            {
                AppSetting app = new AppSetting
                {
                    NewsAm = new NewsAm
                    {
                        ArmenianNews = $"eng/news/allregions/allthemes/{firstDay.Year}/{firstDay.Month:D2}/{firstDay.Day:D2}",
                        BaseAddress = "https://news.am/"
                    }
                };

                Parser parser = new Parser(app);
                var result = parser.ParseAsync(NewsCategoryEnum.ArmenianDiaspora);

                await result.ForEachAsync(item => {
                    Console.WriteLine(item.PublishedDateTimeUtc);
                    listOfItemns.Add(item);
                });
                firstDay = firstDay.AddDays(1);
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed.Seconds);
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
