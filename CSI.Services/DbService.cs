using CSI.Common.Wesco;
using CSI.Data;
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
                var batchId = ctx.Staging_ProductExtract
                    .Select(x => x.Batch)
                    .DefaultIfEmpty(0)
                    .Max();

                return batchId + 1;
            }
        }

        public bool SaveProducts(List<Product> products, int batchId)
        {
            try
            {
                _bgWorker.ReportProgress(0, $"Saving {products.Count} products to database using BatchId {batchId}.");

                var stagingProducts = GetStagingProducts(products, batchId);

                using (var ctx = new ScrapperContext())
                {
                    ctx.Staging_ProductExtract.AddRange(stagingProducts);
                    ctx.SaveChanges();

                    return true;
                }
            }
            catch (Exception e)
            {
                _bgWorker.ReportProgress(0, $"Error while saving products to database: {e.Message}");
                return false;
            }
        }

        private static IEnumerable<Staging_ProductExtract> GetStagingProducts(IEnumerable<Product> products, int batchId)
        {
            var stagingProducts = products
                .Where(p => p.Name != null && p.Price != null)
                .Select(x => new Staging_ProductExtract
                {
                    Batch = batchId,
                    BatchDate = DateTime.UtcNow,

                    ProductId = x.ProductId,
                    ProductName = x.Name,
                    ProductPrice = x.Price
                })
                .ToList();

            return stagingProducts;
        }
    }
}
