using CSI.Common.Enums;
using CSI.FileScraping.Services;
using CSI.WebScraping.Models.Wesco;
using CSI.WebScraping.Services.Wesco;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace CSI.Scrapper
{
    public partial class Main : Form
    {
        private readonly ObservableCollection<Product> _products;
        private readonly BindingSource _bindingSource;
        private SearchAction _searchAction;

        public Main()
        {
            InitializeComponent();

            _products = new ObservableCollection<Product>();
            _products.CollectionChanged += Products_CollectionChanged;

            _bindingSource = new BindingSource { DataSource = _products, AllowNew = false };

            // Error handling for application
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CrashHandler;
            Application.ThreadException += CrashHandler_thread;
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

        private void ResetBindingSourceBindings()
        {
            _bindingSource.ResetBindings(false);
        }

        #region Crash handler implementation

        private static void CrashHandler(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Program Error:" + e);
        }

        private static void CrashHandler_thread(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show("Thread Error: " + e);
        }

        #endregion

        private void Main_Load(object sender, EventArgs e)
        {
            //txtSearchTerm.Text = "01004-001,01155-001,01241-001,012T88-33180-A3,01473-001,01621-001";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            _searchAction = SearchAction.Web;

            if (string.IsNullOrWhiteSpace(txtSearchTerm.Text))
            {
                MessageBox.Show("Please enter comma separated product Id.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PreStepsBeforeSearching();
            bgWebWorker.RunWorkerAsync(txtSearchTerm.Text);
        }

        private void PreStepsBeforeSearching()
        {
            btnSearchWeb.Text = "Searching...";
            btnSearchWeb.Enabled = false;

            txtLogs.Text = string.Empty;
            tsLblStatus.Text = "Please wait! Searching products...";
            tsLblStatus.Image = Properties.Resources.BlueLoader;

            gvProducts.DataSource = _bindingSource;
        }

        private void bgWebWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_searchAction == SearchAction.Web)
                PopulateProductsFromWeb(sender, e.Argument);
            else if (_searchAction == SearchAction.File)
                PopulateProductsFromFile(sender, e.Argument);
        }

        private void PopulateProductsFromWeb(object sender, object arg)
        {
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

        private void bgWebWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // TODO: Pass TextLogs for logging and use UserState for displaying progress

            if (e.UserState != null)
                txtLogs.AppendText(e.UserState + Environment.NewLine + Environment.NewLine);
        }

        private void bgWebWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            tsLblStatus.Text = "Ready";
            tsLblStatus.Image = null;

            btnSearchWeb.Text = "Search";
            btnSearchWeb.Enabled = true;
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFilePath.Text = openFileDialog.FileName;
                btnSearchFile.Enabled = !string.IsNullOrWhiteSpace(txtFilePath.Text);
            }
        }

        private void btnSearchFile_Click(object sender, EventArgs e)
        {
            _searchAction = SearchAction.File;

            if (string.IsNullOrWhiteSpace(txtFilePath.Text))
            {
                MessageBox.Show("Please select a file to search.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            PreStepsBeforeSearching();
            bgWebWorker.RunWorkerAsync();
        }

        private void PopulateProductsFromFile(object sender, object arg)
        {
            //var productIds = arg is string productIdTxt
            //    ? productIdTxt.Split(',').Select(x => x.Trim()).ToList()
            //    : new List<string>();

            var fileService = new FileService(sender as BackgroundWorker);
            fileService.ReadPdfAndGenerateExcelFile(txtFilePath.Text);

            //foreach (var wsProduct in wsProducts)
            //{
            //    _products.Add(wsProduct);
            //}
        }
    }
}
