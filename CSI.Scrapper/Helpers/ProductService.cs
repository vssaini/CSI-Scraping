using CSI.Common.Enums;
using CSI.Common.Wesco;
using CSI.FileScraping.Services;
using CSI.Services;
using CSI.WebScraping.Services.Wesco;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

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

            //_gvProducts.FirstDisplayedScrollingRowIndex = _gvProducts.RowCount - 1;
            SaveProductsToDb();
        }

        private void SaveProductsToDb()
        {
            if (_products.Count <= 100 * _multiplicand)
                return;

            var productsToSave = _products.Skip(_rowsSaved).Take(100);
            _dbService.SaveProducts(productsToSave, _batchId);

            _rowsSaved += 100;
            _multiplicand++;
        }

        public void PopulateProducts(SearchAction searchAction, object arg)
        {
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

            var ws = new WescoService(_bgWorker);
            var wsProducts = ws.GetProducts(productIds);

            foreach (var wsProduct in wsProducts)
            {
                _products.Add(wsProduct);
            }
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

            _bgWorker.ReportProgress(0, $"Retrieving product Ids from excel file.");

            var fileService = new FileService(_bgWorker);
            var productIds = fileService.GetProductIdsFromExcelFile(excelFilePath);

            var ws = new WescoService(_bgWorker);
            var wsProducts = ws.GetProducts(productIds);

            foreach (var wsProduct in wsProducts)
            {
                _products.Add(wsProduct);
            }
        }
    }
}
