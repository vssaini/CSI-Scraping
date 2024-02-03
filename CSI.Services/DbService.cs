using CSI.Common;
using CSI.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CSI.Services
{
    public class DbService
    {
        private readonly BackgroundWorker _bgWorker;

        public DbService(BackgroundWorker bgWorker)
        {
            _bgWorker = bgWorker;
        }

        public int GetBatchId()
        {
            using (var ctx = new ScrapperContext())
            {
                var batchId = ctx.Products
                    .Select(x => x.Batch)
                    .DefaultIfEmpty(0)
                    .Max();

                return batchId + 1;
            }
        }

        public bool SaveProducts(List<ProductDto> products, int batchId)
        {
            try
            {
                _bgWorker.ReportProgress(0, $"Initiating saving of {products.Count} products. Products without name or price will not be saved.");

                var stagingProducts = GetDbProducts(products, batchId);
                if (stagingProducts.Count == 0)
                {
                    _bgWorker.ReportProgress(0, "No products found with name and price for saving.");
                    return false;
                }

                _bgWorker.ReportProgress(0, $"Found {stagingProducts.Count}/{products.Count} products with name and price for saving.");

                using var ctx = new ScrapperContext();
                ctx.Products.AddRange(stagingProducts);
                ctx.SaveChanges();

                _bgWorker.ReportProgress(0, $"Saved {stagingProducts.Count} products to database using batch id {batchId}.");

                return true;
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                    e = e.InnerException;

                Log.Logger.Error(e, $"Error while saving products to database: {e.Message}");
                _bgWorker.ReportProgress(0, $"Error while saving products to database: {e.Message}");

                return false;
            }
        }

        private static List<Product> GetDbProducts(IEnumerable<ProductDto> products, int batchId)
        {
            var stagingProducts = products
                .Where(p => p.Name != null && p.Price > 0)
                .Select(p => new Product
                {
                    Batch = batchId,
                    BatchDate = DateTime.UtcNow,

                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    ProductPrice = p.Price,
                    ProductStock = p.Stock,

                    Source = p.Source
                })
                .ToList();

            return stagingProducts;
        }
    }
}
