using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using CSI.Common;
using CSI.WebScraping.Models.ScrapeMe;
using CsvHelper;
using HtmlAgilityPack;

namespace CSI.WebScraping.Services.ScrapeMe
{
    internal class HapService
    {
        public static void GetPokemonProductsAndExportToCSV()
        {
            Logger.LogWithDoubleLines($"Scraping of Pokemon products from Scrapeme.live started at {DateTime.Now:HH:mm:ss}.");

            var pokemonProducts = GetAllPokemonProducts();

            Logger.LogSuccess($"Scraping of Pokemon products from Scrapeme.live completed at {DateTime.Now:HH:mm:ss}.");

            ExportProductsToCSV(pokemonProducts);
        }

        private static IEnumerable<PokemonProduct> GetAllPokemonProducts()
        {
            // the URL of the first pagination web page 
            const string firstPageToScrape = "https://scrapeme.live/shop/page/1/";

            // the list of pages discovered during the crawling task 
            var pagesDiscovered = new List<string> { firstPageToScrape };

            // the list of pages that remains to be scraped 
            var pagesToScrape = new Queue<string>();

            // initializing the list with firstPageToScrape 
            pagesToScrape.Enqueue(firstPageToScrape);

            // current crawling iteration 
            int i = 1;

            // the maximum number of pages to scrape before stopping 
            const int limit = 48;

            var allProducts = new List<PokemonProduct>();

            var web = GetHtmlWeb();

            // until there are no pages to scrape or limit is hit 
            while (pagesToScrape.Count != 0 && i < limit)
            {
                // extracting the current page to scrape from the queue 
                var currentPage = pagesToScrape.Dequeue();

                Console.WriteLine($"Crawling page {i} - {currentPage}");

                // loading the page 
                var currentDocument = web.Load(currentPage);

                // selecting the list of pagination HTML elements 
                var paginationHtmlElements = currentDocument.DocumentNode.QuerySelectorAll("a.page-numbers");

                // to avoid visiting a page twice 
                foreach (var paginationHtmlElement in paginationHtmlElements)
                {
                    // extracting the current pagination URL 
                    var newPaginationLink = paginationHtmlElement.Attributes["href"].Value;

                    // if the page discovered is new 
                    if (!pagesDiscovered.Contains(newPaginationLink))
                    {
                        // if the page discovered needs to be scraped 
                        if (!pagesToScrape.Contains(newPaginationLink))
                        {
                            pagesToScrape.Enqueue(newPaginationLink);
                        }
                        pagesDiscovered.Add(newPaginationLink);
                    }
                }

                var products = GetPokemonProducts(currentPage, web);
                allProducts.AddRange(products);

                // incrementing the crawling counter 
                i++;
            }

            return allProducts;
        }

        private static IEnumerable<PokemonProduct> GetPokemonProducts(string url, HtmlWeb web)
        {
            var sw = new Stopwatch();
            sw.Start();

            var document = web.Load(url);

            var pokemonProducts = new List<PokemonProduct>();

            // selecting all HTML product elements from the current page 
            var productHtmlElements = document.DocumentNode.QuerySelectorAll("li.product");

            // iterating over the list of product elements 
            foreach (var prodHtmlEle in productHtmlElements)
            {
                // scraping the interesting data from the current HTML element 
                var prodUrl = HtmlEntity.DeEntitize(prodHtmlEle.QuerySelector("a").Attributes["href"].Value);
                var image = HtmlEntity.DeEntitize(prodHtmlEle.QuerySelector("img").Attributes["src"].Value);
                var name = HtmlEntity.DeEntitize(prodHtmlEle.QuerySelector("h2").InnerText);
                var price = HtmlEntity.DeEntitize(prodHtmlEle.QuerySelector(".price").InnerText);

                // instancing a new PokemonProduct object 
                var pokemonProduct = new PokemonProduct { Url = prodUrl, Image = image, Name = name, Price = price };

                // adding the object containing the scraped data to the list 
                pokemonProducts.Add(pokemonProduct);
            }

            sw.Stop();

            Logger.LogInfo(sw.Elapsed.Seconds > 60
                ? $"Found {pokemonProducts.Count} products in {sw.Elapsed.Minutes}m {sw.Elapsed.Seconds}s"
                : $"Found {pokemonProducts.Count} products in {sw.Elapsed.Seconds}s");

            return pokemonProducts;
        }

        private static void ExportProductsToCSV(IEnumerable<PokemonProduct> pokemonProducts)
        {
            using var writer = new StreamWriter("pokemon-products.csv");
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            // populating the CSV file 
            csv.WriteRecords(pokemonProducts);
        }

        private static HtmlWeb GetHtmlWeb()
        {
            var web = new HtmlWeb();

            // Ref - https://www.zenrows.com/blog/web-scraping-c-sharp#avoid-being-blocked
            // Avoid being blocked by setting a User-Agent header
            // setting a global User-Agent header in HAP 
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36";

            return web;
        }
    }
}
