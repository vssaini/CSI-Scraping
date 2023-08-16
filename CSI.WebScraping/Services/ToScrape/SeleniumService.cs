using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CSI.WebScraping.Models.ToScrape;
using CsvHelper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace CSI.WebScraping.Services.ToScrape
{
    internal class SeleniumService
    {
        public static void GetQuotesAndExportToCSV()
        {
            // setting up the driver
            new DriverManager().SetUpDriver(new ChromeConfig());

            var driver = new ChromeDriver();
            // load the webpage
            driver.Navigate().GoToUrl("http://quotes.toscrape.com/js/");

            var quotes = new List<Quote>();
            var quoteContainers = driver.FindElements(By.CssSelector("div.quote"));
            foreach (var item in quoteContainers)
            {
                Quote quote = new()
                {
                    Text = item.FindElement(By.CssSelector("span.text")).Text,
                    Author = item.FindElement(By.CssSelector(".author")).Text
                };
                quotes.Add(quote);
                Console.WriteLine(quote.ToString());
            }


            using (var writer = new StreamWriter("./quotes.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(quotes);
            }

            // closing the browser 
            driver.Quit();
        }
    }
}
