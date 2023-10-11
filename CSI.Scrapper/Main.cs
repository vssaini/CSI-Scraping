using CSI.Common.Enums;
using CSI.Scrapper.Helpers;
using CSI.Scrapper.Properties;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace CSI.Scrapper
{
    public partial class Main : Form
    {
        private ProductService _prodService;
        private SearchAction _searchAction;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            _prodService = new ProductService(bgWorker, gvProducts) { MainAssemblyLocation = Assembly.GetExecutingAssembly().Location };
        }

        private void bgWebWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _prodService.CreateScreenshotsDirectory();
            _prodService.PopulateProducts(_searchAction, e.Argument);
        }

        private void bgWebWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
                txtLogs.AppendText(e.UserState + Environment.NewLine + Environment.NewLine);
        }

        private void bgWebWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ResetControlsState(e);
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

        #region Helper Methods

        private void UpdateControlsStateBeforeSearching()
        {
            txtLogs.Text = string.Empty;
            tsLblStatus.Image = Resources.BlueLoader;

            btnSearchProduct.Enabled = btnSearchPdfProduct.Enabled = btnSearchExcelProduct.Enabled =
                btnBrowsePdfFile.Enabled = btnBrowseExcelFile.Enabled = false;

            gvProducts.DataSource = _prodService.BindingSource;

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

        private void ResetControlsState(AsyncCompletedEventArgs e)
        {
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

            btnSearchProduct.Text = btnSearchPdfProduct.Text = btnSearchExcelProduct.Text = Resources.BtnSearchDefaultTxt;

            btnSearchProduct.Enabled = btnSearchPdfProduct.Enabled = btnSearchExcelProduct.Enabled =
                btnBrowsePdfFile.Enabled = btnBrowseExcelFile.Enabled = true;
        }

        #endregion
    }
}
