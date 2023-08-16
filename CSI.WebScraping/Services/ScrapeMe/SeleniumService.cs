using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using CSI.Common;
using CSI.WebScraping.Models.ScrapeMe;
using CsvHelper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace CSI.WebScraping.Services.ScrapeMe
{
    public static class SeleniumService
    {
        public static void GetPokemonProductsAndExportToCSV()
        {
            Logger.LogWithDoubleLines($"Scraping Pokemon products from Scrapeme.live started at {DateTime.Now:HH:mm:ss}.");

            var pokemonProducts = GetAllPokemonProducts();

            Logger.LogSuccess($"Scraping Pokemon products from Scrapeme.live completed at {DateTime.Now:HH:mm:ss}.");

            ExportProductsToCSV(pokemonProducts);
        }

        private static IEnumerable<PokemonProduct> GetAllPokemonProducts()
        {
            var sw = new Stopwatch();
            sw.Start();

            var pokemonProducts = new List<PokemonProduct>();

            // to open Chrome in headless mode 
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");

            // starting a Selenium instance 
            using (var driver = new ChromeDriver(chromeOptions))
            {
                // navigating to the target page in the browser 
                driver.Navigate().GoToUrl("https://scrapeme.live/shop/");

                var screenShot = driver.GetScreenshot();
                screenShot.SaveAsFile("Screenshot_ScrapeMe.png");

                // getting the HTML product elements 
                var productHtmlElements = driver.FindElements(By.CssSelector("li.product"));
                // iterating over them to scrape the data of interest 
                foreach (var productHtmlElement in productHtmlElements)
                {
                    // scraping logic 
                    var url = productHtmlElement.FindElement(By.CssSelector("a")).GetAttribute("href");
                    var image = productHtmlElement.FindElement(By.CssSelector("img")).GetAttribute("src");
                    var name = productHtmlElement.FindElement(By.CssSelector("h2")).Text;
                    var price = productHtmlElement.FindElement(By.CssSelector(".price")).Text;

                    var pokemonProduct = new PokemonProduct { Url = url, Image = image, Name = name, Price = price };

                    pokemonProducts.Add(pokemonProduct);
                }
            }

            Logger.LogInfo(sw.Elapsed.Seconds > 60
                ? $"Found {pokemonProducts.Count} products in {sw.Elapsed.Minutes}m {sw.Elapsed.Seconds}s"
                : $"Found {pokemonProducts.Count} products in {sw.Elapsed.Seconds}s");

            return pokemonProducts;
        }

        private static void ExportProductsToCSV(IEnumerable<PokemonProduct> pokemonProducts)
        {
            using (var writer = new StreamWriter("pokemon-products.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(pokemonProducts);
            }
        }
    }
}
