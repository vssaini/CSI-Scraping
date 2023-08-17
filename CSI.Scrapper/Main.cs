using CSI.Common.Enums;
using CSI.Common.Wesco;
using CSI.FileScraping.Services;
using CSI.Scrapper.Properties;
using CSI.Services;
using CSI.WebScraping.Services.Wesco;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace CSI.Scrapper
{
    public partial class Main : Form
    {
        private DbService _dbService;

        private ObservableCollection<Product> _products;
        private BindingSource _bindingSource;

        private SearchAction _searchAction;
        private int _batchId;
        private int _rowsSaved;

        public Main()
        {
            InitializeComponent();

            ConfigureGlobalErrorHandling();

            InitializeVariables();
        }

        #region Global error handling

        private static void ConfigureGlobalErrorHandling()
        {
            // Error handling for application
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CrashHandler;
            Application.ThreadException += CrashHandler_thread;
        }

        private static void CrashHandler(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(Resources.CrashProgramError + " " + e, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CrashHandler_thread(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(Resources.CrashThreadError + " " + e, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        private void Main_Load(object sender, EventArgs e)
        {
            InitializeVariables();

            _batchId = _dbService.GetBatchId();
            //txtSearchTerm.Text = "01004-001,01155-001,01241-001,012T88-33180-A3,01473-001,01621-001";
        }

        private void InitializeVariables()
        {
            _dbService = new DbService(bgWorker);

            _products = new ObservableCollection<Product>();
            _products.CollectionChanged += Products_CollectionChanged;

            _bindingSource = new BindingSource { DataSource = _products, AllowNew = false };
        }

        private void Products_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // To access control cross-thread
            if (gvProducts.InvokeRequired)
            {
                gvProducts.Invoke(new Action(ResetBindingSourceBindings));
                return;
            }

            ResetBindingSourceBindings();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            _searchAction = SearchAction.Web;

            if (string.IsNullOrWhiteSpace(txtSearchTerm.Text))
            {
                MessageBox.Show(Resources.ErrorMsgForMissingProductIds, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateControlsStateBeforeSearching();
            bgWorker.RunWorkerAsync(txtSearchTerm.Text);
        }

        private void bgWebWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (_searchAction)
            {
                case SearchAction.Web:
                    PopulateProductsFromWeb(sender, e.Argument);
                    break;

                case SearchAction.Pdf:
                    PopulateProductsFromPdfFile(sender, e.Argument);
                    break;

                case SearchAction.Excel:
                    PopulateProductsFromExcelFile(e.Argument);
                    break;
            }
        }

        private void bgWebWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
                txtLogs.AppendText(e.UserState + Environment.NewLine + Environment.NewLine);
        }

        private void bgWebWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tsLblStatus.Image = null;

            if (e.Error != null)
            {
                txtLogs.AppendText("ERROR - " + e.Error);

                tsLblStatus.ForeColor = System.Drawing.Color.Red;
                tsLblStatus.Text = Resources.StatusError;
                tsLblStatus.Image = Resources.Error;
            }
            else
            {
                tsLblStatus.ForeColor = System.Drawing.Color.Black;
                tsLblStatus.Text = Resources.StatusReady;
                tsLblStatus.Image = null;
            }

            ResetControlsState();
        }

        private void btnBrowsePdfFile_Click(object sender, EventArgs e)
        {
            if (openPdfDialog.ShowDialog() == DialogResult.OK)
            {
                txtPdfFilePath.Text = openPdfDialog.FileName;
                btnSearchPdfProduct.Enabled = !string.IsNullOrWhiteSpace(txtPdfFilePath.Text);
            }
        }

        private void btnSearchPdfProductFile_Click(object sender, EventArgs e)
        {
            _searchAction = SearchAction.Pdf;

            if (string.IsNullOrWhiteSpace(txtPdfFilePath.Text))
            {
                MessageBox.Show(Resources.ErrorMsgForMissingFile, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateControlsStateBeforeSearching();
            bgWorker.RunWorkerAsync(txtPdfFilePath.Text);
        }

        private void tsBtnInfo_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.Show();
        }

        private void btnBrowseExcelFile_Click(object sender, EventArgs e)
        {
            if (openExcelDialog.ShowDialog() == DialogResult.OK)
            {
                txtExcelFilePath.Text = openExcelDialog.FileName;
                btnSearchExcelProduct.Enabled = !string.IsNullOrWhiteSpace(txtExcelFilePath.Text);
            }
        }

        private void btnSearchExcelProduct_Click(object sender, EventArgs e)
        {
            _searchAction = SearchAction.Excel;

            if (string.IsNullOrWhiteSpace(txtExcelFilePath.Text))
            {
                MessageBox.Show(Resources.ErrorMsgForMissingFile, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateControlsStateBeforeSearching();
            bgWorker.RunWorkerAsync(txtExcelFilePath.Text);
        }

        #region Helper Methods

        private void ResetBindingSourceBindings()
        {
            _bindingSource.ResetBindings(false);

            SaveProductsToDb();
        }

        private void SaveProductsToDb()
        {
            if (_products.Count % 100 != 0) return;

            var productsToSave = _products.Skip(_rowsSaved).Take(100);
            _dbService.SaveProducts(productsToSave, _batchId);

            _rowsSaved += 100;
        }

        private void UpdateControlsStateBeforeSearching()
        {
            txtLogs.Text = string.Empty;
            tsLblStatus.Image = Resources.BlueLoader;

            btnSearchProduct.Enabled = btnSearchPdfProduct.Enabled = btnSearchExcelProduct.Enabled =
                btnBrowsePdfFile.Enabled = btnBrowseExcelFile.Enabled = false;

            gvProducts.DataSource = _bindingSource;

            switch (_searchAction)
            {
                case SearchAction.Web:
                    btnSearchProduct.Text = Resources.BtnSearchingStatus;
                    tsLblStatus.Text = Resources.InfoSearchProductFromWeb;
                    break;

                case SearchAction.Pdf:
                    btnSearchPdfProduct.Text = Resources.BtnSearchingStatus;
                    tsLblStatus.Text = Resources.InfoSearchProductFromPdf;
                    break;

                case SearchAction.Excel:
                    btnSearchExcelProduct.Text = Resources.BtnSearchingStatus;
                    tsLblStatus.Text = Resources.InfoSearchProductFromExcel;
                    break;
            }
        }

        private void ResetControlsState()
        {
            btnSearchProduct.Enabled = btnSearchPdfProduct.Enabled = btnSearchExcelProduct.Enabled =
                btnBrowsePdfFile.Enabled = btnBrowseExcelFile.Enabled = true;
        }

        private void PopulateProductsFromWeb(object sender, object arg)
        {
            _products.Clear();

            var productIds = arg is string productIdTxt
                ? productIdTxt.Split(',').Select(x => x.Trim()).ToList()
                : new List<string>();

            var ws = new WescoService(sender as BackgroundWorker);
            var wsProducts = ws.GetProducts(productIds);

            foreach (var wsProduct in wsProducts)
            {
                _products.Add(wsProduct);
            }
        }

        private void PopulateProductsFromPdfFile(object sender, object arg)
        {
            _products.Clear();

            var filePath = arg as string ?? string.Empty;

            var fileService = new FileService(sender as BackgroundWorker, Assembly.GetExecutingAssembly().Location);
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

            bgWorker.ReportProgress(0, $"Retrieving product Ids from excel file.");

            var fileService = new FileService(bgWorker, Assembly.GetExecutingAssembly().Location);
            var productIds = fileService.GetProductIdsFromExcelFile(excelFilePath);

            var ws = new WescoService(bgWorker);
            var wsProducts = ws.GetProducts(productIds);

            foreach (var wsProduct in wsProducts)
            {
                _products.Add(wsProduct);
            }
        }
        
        #endregion
    }
}
