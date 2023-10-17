using CSI.Common.Enums;
using CSI.Common.Wesco;
using CSI.FileScraping.Services;
using CSI.Services;
using CSI.WebScraping.Services.Wesco;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Windows.Forms;
using CSI.WebScraping.Services.ScanSource;

namespace CSI.Scrapper.Helpers
{
    internal class ProductService
    {
        // Controls
        private readonly BackgroundWorker _bgWorker;
        private readonly DataGridView _gvProducts;

        private readonly DbService _dbService;
        private readonly ObservableCollection<Product> _products;
        private readonly BindingSource _bindingSource;
        private int _batchId, _rowsSaved, _multiplicand = 1;
        private static int _recordsToSaveInBatch;

        public ProductService(BackgroundWorker bgWorker, DataGridView gvProducts)
        {
            _bgWorker = bgWorker;

            _gvProducts = gvProducts;
            _dbService = new DbService(bgWorker);

            _products = new ObservableCollection<Product>();
            _products.CollectionChanged += Products_CollectionChanged;

            _bindingSource = new BindingSource { DataSource = _products, AllowNew = false };

            InitVariables();
        }

        /// <summary>
        /// Gets or sets the main assembly location.
        /// </summary>
        public string MainAssemblyLocation { get; set; }

        /// <summary>
        /// Gets or sets the binding source of products.
        /// </summary>
        public BindingSource BindingSource => _bindingSource;

        private void InitVariables()
        {
            _recordsToSaveInBatch = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfRecordsToSaveInBatch"]);
            _batchId = _dbService.GetBatchId();
        }

        private void Products_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // To access control cross-thread
            if (_gvProducts.InvokeRequired)
            {
                _gvProducts.Invoke(new Action(ResetBindingSourceBindings));
                return;
            }

            ResetBindingSourceBindings();
        }

        private void ResetBindingSourceBindings()
        {
            _bindingSource.ResetBindings(false);

            SetGridScrollBarToBottom();
            SaveProductsToDb();
        }

        private void SaveProductsToDb(bool saveInBatch = true)
        {
            return;

            List<Product> productsToSave;

            if (saveInBatch)
            {
                if (_products.Count <= _recordsToSaveInBatch * _multiplicand)
                    return;

                productsToSave = _products.Skip(_rowsSaved).Take(_recordsToSaveInBatch).ToList();
                _dbService.SaveProducts(productsToSave, _batchId);

                _rowsSaved += _recordsToSaveInBatch;
                _multiplicand++;
            }
            else
            {
                var rowsLeftToSave = _products.Count - _rowsSaved;

                productsToSave = _products.Skip(_rowsSaved).Take(rowsLeftToSave).ToList();
                _dbService.SaveProducts(productsToSave, _batchId);

                _rowsSaved = 0;
                _multiplicand = 1;
            }
        }

        private void SetGridScrollBarToBottom()
        {
            if (_gvProducts.RowCount > 0)
                _gvProducts.FirstDisplayedScrollingRowIndex = _gvProducts.RowCount - 1;
        }

        public void PopulateProducts(SearchAction searchAction, object arg)
        {
            // TODO: Brainstorm if we should start scraping on all three via three threads

            Log.Logger.Information("Fetching products via search action {SearchAction}", searchAction);

            switch (searchAction)
            {
                case SearchAction.Web:
                    PopulateProductsFromWeb(arg);
                    break;

                case SearchAction.Pdf:
                    PopulateProductsFromPdfFile(arg);
                    break;

                case SearchAction.Excel:
                    PopulateProductsFromExcelFile(arg);
                    break;
            }
        }

        private void PopulateProductsFromWeb(object arg)
        {
            _products.Clear();

            var productIds = arg is string productIdTxt
                ? productIdTxt.Split(',').Select(x => x.Trim()).ToList()
                : new List<string>();

            //PopulateProductsFromWesco(productIds);
            PopulateProductsFromScanSource(productIds);
        }

        private void PopulateProductsFromPdfFile(object arg)
        {
            _products.Clear();

            var filePath = arg as string ?? string.Empty;

            var fileService = new FileService(_bgWorker) { MainAssemblyLocation = MainAssemblyLocation };
            var products = fileService.GetProducts(filePath);

            foreach (var product in products)
            {
                _products.Add(product);
            }
        }

        private void PopulateProductsFromExcelFile(object arg)
        {
            _products.Clear();

            var excelFilePath = arg as string ?? string.Empty;

            _bgWorker.ReportProgress(0, "Retrieving product Ids from excel file.");

            var fileService = new FileService(_bgWorker);
            var productIds = fileService.GetProductIdsFromExcelFile(excelFilePath);

            //PopulateProductsFromWesco(productIds);
            PopulateProductsFromScanSource(productIds);
            SaveProductsToDb(false);
        }

        private void PopulateProductsFromWesco(List<string> productIds)
        {
            var ws = new WescoService(_bgWorker);
            var wsProducts = ws.GetProducts(productIds);

            foreach (var wsProduct in wsProducts)
            {
                _products.Add(wsProduct);
            }
        }

        private void PopulateProductsFromScanSource(List<string> productIds)
        {
            var ws = new ScanService(_bgWorker);
            var wsProducts = ws.GetProducts(productIds);

            foreach (var wsProduct in wsProducts)
            {
                _products.Add(wsProduct);
            }
        }
    }
}
