using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CSI.WebScraping.Models.ToScrape;
using CsvHelper;
using HtmlAgilityPack;

namespace CSI.WebScraping.Services.ToScrape
{
    internal class HapService
    {
        public static void GetBooksAndExportToCSV()
        {
            const string url = "http://books.toscrape.com/catalogue/category/books/mystery_3/index.html";

            var bookLinks = GetBookLinks(url);
            Console.WriteLine("Found {0} links", bookLinks.Count);

            var books = GetBookDetails(bookLinks);
            ExportToCSV(books);
        }

        private static List<string> GetBookLinks(string url)
        {
            var bookLinks = new List<string>();
            var doc = GetDocument(url);
            HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes("//h3/a");

            var baseUri = new Uri(url);
            foreach (var link in linkNodes)
            {
                string href = link.Attributes["href"].Value;
                bookLinks.Add(new Uri(baseUri, href).AbsoluteUri);
            }
            return bookLinks;
        }

        // Parses the URL and returns HtmlDocument object
        private static HtmlDocument GetDocument(string url)
        {
            var web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            return doc;
        }

        private static IEnumerable<Book> GetBookDetails(List<string> urls)
        {
            var books = new List<Book>();
            foreach (var url in urls)
            {
                HtmlDocument document = GetDocument(url);
                var titleXPath = "//h1";
                var priceXPath = "//div[contains(@class,\"product_main\")]/p[@class=\"price_color\"]";
                var book = new Book();
                book.Title = document.DocumentNode.SelectSingleNode(titleXPath).InnerText;
                book.Price = document.DocumentNode.SelectSingleNode(priceXPath).InnerText;
                books.Add(book);
            }
            return books;
        }

        private static void ExportToCSV(IEnumerable<Book> books)
        {
            using var writer = new StreamWriter("./books.csv");
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(books);
        }
    }
}
