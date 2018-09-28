using NewsAmParser;
using NewsAmParser.DataStructure;
using System;
using System.Collections.Async;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

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
            Parser parser = new Parser();
            while (currentDate >= firstDay)
            {
                var result = parser.ParseAsync("https://news.am/", $"eng/news/allregions/allthemes/{firstDay.Year}/{firstDay.Month:D2}/{firstDay.Day:D2}");

                await result.ForEachAsync(item => {
                    Console.WriteLine(item.PublishedDateTimeUtc);
                    listOfItemns.Add(item);
                });
                firstDay = firstDay.AddDays(1);
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed.Seconds);
            Console.WriteLine($"Failed Message count {parser.FailedArticleCount}");
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
