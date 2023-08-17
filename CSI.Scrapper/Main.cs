using CSI.Common.Enums;
using CSI.Common.Wesco;
using CSI.FileScraping.Services;
using CSI.Scrapper.Properties;
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
            MessageBox.Show(Resources.CrashProgramError + " " + e);
        }

        private static void CrashHandler_thread(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(Resources.CrashThreadError + " " + e);
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
                MessageBox.Show(Resources.ErrorMsgForMissingProductIds, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateControlsStateBeforeSearching();
            bgWebWorker.RunWorkerAsync(txtSearchTerm.Text);
        }

        private void UpdateControlsStateBeforeSearching()
        {
            txtLogs.Text = string.Empty;
            tsLblStatus.Image = Resources.BlueLoader;
            gvProducts.DataSource = _bindingSource;

            if (_searchAction == SearchAction.File)
            {
                btnSearchFile.Text = Resources.BtnSearchingStatus;
                btnSearchFile.Enabled = false;
                tsLblStatus.Text = Resources.InfoSearchProductFromPdf;
            }
            else if (_searchAction == SearchAction.Web)
            {
                btnSearchWeb.Text = Resources.BtnSearchingStatus;
                btnSearchWeb.Enabled = false;
                tsLblStatus.Text = Resources.InfoSearchProductFromWeb;
            }
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

                tsLblStatus.Image = Resources.Error;
                tsLblStatus.Text = "Error occurred.";
            }
            else
            {
                tsLblStatus.Text = Resources.StatusReady;
                tsLblStatus.Image = null;

                btnSearchWeb.Text = btnSearchFile.Text = Resources.BtnSearchDefaultTxt;
                btnSearchWeb.Enabled = btnSearchFile.Enabled = true;
            }
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
                MessageBox.Show(Resources.ErrorMsgForMissingFile, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateControlsStateBeforeSearching();
            bgWebWorker.RunWorkerAsync(txtFilePath.Text);
        }

        private void PopulateProductsFromFile(object sender, object arg)
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
    }
}
