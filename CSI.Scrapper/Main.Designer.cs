namespace CSI.Scrapper
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.lblEnterSearchTerm = new System.Windows.Forms.Label();
            this.txtSearchTerm = new System.Windows.Forms.TextBox();
            this.gbSearchWeb = new System.Windows.Forms.GroupBox();
            this.btnBrowseExcelFile = new System.Windows.Forms.Button();
            this.btnSearchExcelProduct = new System.Windows.Forms.Button();
            this.txtExcelFilePath = new System.Windows.Forms.TextBox();
            this.lblSelectProduct = new System.Windows.Forms.Label();
            this.btnSearchProduct = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsLblStatus = new System.Windows.Forms.ToolStripLabel();
            this.tsBtnInfo = new System.Windows.Forms.ToolStripButton();
            this.gvProducts = new System.Windows.Forms.DataGridView();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.gbSearchPdf = new System.Windows.Forms.GroupBox();
            this.btnSearchPdfProduct = new System.Windows.Forms.Button();
            this.btnBrowsePdfFile = new System.Windows.Forms.Button();
            this.lblSelectFile = new System.Windows.Forms.Label();
            this.txtPdfFilePath = new System.Windows.Forms.TextBox();
            this.openPdfDialog = new System.Windows.Forms.OpenFileDialog();
            this.openExcelDialog = new System.Windows.Forms.OpenFileDialog();
            this.gbSearchWeb.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvProducts)).BeginInit();
            this.gbSearchPdf.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblEnterSearchTerm
            // 
            this.lblEnterSearchTerm.AutoSize = true;
            this.lblEnterSearchTerm.Location = new System.Drawing.Point(16, 35);
            this.lblEnterSearchTerm.Name = "lblEnterSearchTerm";
            this.lblEnterSearchTerm.Size = new System.Drawing.Size(198, 15);
            this.lblEnterSearchTerm.TabIndex = 1;
            this.lblEnterSearchTerm.Text = "Enter comma separated product ids:";
            // 
            // txtSearchTerm
            // 
            this.txtSearchTerm.Location = new System.Drawing.Point(220, 27);
            this.txtSearchTerm.Name = "txtSearchTerm";
            this.txtSearchTerm.Size = new System.Drawing.Size(190, 23);
            this.txtSearchTerm.TabIndex = 2;
            // 
            // gbSearchWeb
            // 
            this.gbSearchWeb.Controls.Add(this.btnBrowseExcelFile);
            this.gbSearchWeb.Controls.Add(this.btnSearchExcelProduct);
            this.gbSearchWeb.Controls.Add(this.txtExcelFilePath);
            this.gbSearchWeb.Controls.Add(this.lblSelectProduct);
            this.gbSearchWeb.Controls.Add(this.btnSearchProduct);
            this.gbSearchWeb.Controls.Add(this.lblEnterSearchTerm);
            this.gbSearchWeb.Controls.Add(this.txtSearchTerm);
            this.gbSearchWeb.Location = new System.Drawing.Point(12, 12);
            this.gbSearchWeb.Name = "gbSearchWeb";
            this.gbSearchWeb.Size = new System.Drawing.Size(518, 102);
            this.gbSearchWeb.TabIndex = 3;
            this.gbSearchWeb.TabStop = false;
            this.gbSearchWeb.Text = "Search web for product price";
            // 
            // btnBrowseExcelFile
            // 
            this.btnBrowseExcelFile.Location = new System.Drawing.Point(335, 63);
            this.btnBrowseExcelFile.Name = "btnBrowseExcelFile";
            this.btnBrowseExcelFile.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseExcelFile.TabIndex = 5;
            this.btnBrowseExcelFile.Text = "Browse ...";
            this.btnBrowseExcelFile.UseVisualStyleBackColor = true;
            this.btnBrowseExcelFile.Click += new System.EventHandler(this.btnBrowseExcelFile_Click);
            // 
            // btnSearchExcelProduct
            // 
            this.btnSearchExcelProduct.Enabled = false;
            this.btnSearchExcelProduct.Location = new System.Drawing.Point(425, 64);
            this.btnSearchExcelProduct.Name = "btnSearchExcelProduct";
            this.btnSearchExcelProduct.Size = new System.Drawing.Size(75, 23);
            this.btnSearchExcelProduct.TabIndex = 5;
            this.btnSearchExcelProduct.Text = "Search";
            this.btnSearchExcelProduct.UseVisualStyleBackColor = true;
            this.btnSearchExcelProduct.Click += new System.EventHandler(this.btnSearchExcelProduct_Click);
            // 
            // txtExcelFilePath
            // 
            this.txtExcelFilePath.Location = new System.Drawing.Point(127, 64);
            this.txtExcelFilePath.Name = "txtExcelFilePath";
            this.txtExcelFilePath.Size = new System.Drawing.Size(196, 23);
            this.txtExcelFilePath.TabIndex = 5;
            // 
            // lblSelectProduct
            // 
            this.lblSelectProduct.AutoSize = true;
            this.lblSelectProduct.Location = new System.Drawing.Point(16, 72);
            this.lblSelectProduct.Name = "lblSelectProduct";
            this.lblSelectProduct.Size = new System.Drawing.Size(105, 15);
            this.lblSelectProduct.TabIndex = 4;
            this.lblSelectProduct.Text = "Select product file:";
            // 
            // btnSearchProduct
            // 
            this.btnSearchProduct.Location = new System.Drawing.Point(425, 26);
            this.btnSearchProduct.Name = "btnSearchProduct";
            this.btnSearchProduct.Size = new System.Drawing.Size(75, 23);
            this.btnSearchProduct.TabIndex = 3;
            this.btnSearchProduct.Text = "Search";
            this.btnSearchProduct.UseVisualStyleBackColor = true;
            this.btnSearchProduct.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLblStatus,
            this.tsBtnInfo});
            this.toolStrip1.Location = new System.Drawing.Point(0, 537);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(883, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsLblStatus
            // 
            this.tsLblStatus.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsLblStatus.Name = "tsLblStatus";
            this.tsLblStatus.Size = new System.Drawing.Size(39, 22);
            this.tsLblStatus.Text = "Ready";
            // 
            // tsBtnInfo
            // 
            this.tsBtnInfo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsBtnInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsBtnInfo.Image = global::CSI.Scrapper.Properties.Resources.Info;
            this.tsBtnInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsBtnInfo.Name = "tsBtnInfo";
            this.tsBtnInfo.Size = new System.Drawing.Size(23, 22);
            this.tsBtnInfo.Text = "About CSI Scrapper";
            this.tsBtnInfo.Click += new System.EventHandler(this.tsBtnInfo_Click);
            // 
            // gvProducts
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightCyan;
            this.gvProducts.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gvProducts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvProducts.Location = new System.Drawing.Point(12, 216);
            this.gvProducts.Name = "gvProducts";
            this.gvProducts.ReadOnly = true;
            this.gvProducts.Size = new System.Drawing.Size(518, 304);
            this.gvProducts.TabIndex = 5;
            // 
            // txtLogs
            // 
            this.txtLogs.BackColor = System.Drawing.SystemColors.Info;
            this.txtLogs.Location = new System.Drawing.Point(549, 20);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogs.Size = new System.Drawing.Size(319, 500);
            this.txtLogs.TabIndex = 6;
            this.txtLogs.Text = "Welcome to CSI Scrapper!";
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWebWorker_DoWork);
            this.bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWebWorker_ProgressChanged);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWebWorker_RunWorkerCompleted);
            // 
            // gbSearchPdf
            // 
            this.gbSearchPdf.Controls.Add(this.btnSearchPdfProduct);
            this.gbSearchPdf.Controls.Add(this.btnBrowsePdfFile);
            this.gbSearchPdf.Controls.Add(this.lblSelectFile);
            this.gbSearchPdf.Controls.Add(this.txtPdfFilePath);
            this.gbSearchPdf.Location = new System.Drawing.Point(12, 133);
            this.gbSearchPdf.Name = "gbSearchPdf";
            this.gbSearchPdf.Size = new System.Drawing.Size(518, 64);
            this.gbSearchPdf.TabIndex = 4;
            this.gbSearchPdf.TabStop = false;
            this.gbSearchPdf.Text = "Search file for product price";
            // 
            // btnSearchPdfProduct
            // 
            this.btnSearchPdfProduct.Enabled = false;
            this.btnSearchPdfProduct.Location = new System.Drawing.Point(425, 26);
            this.btnSearchPdfProduct.Name = "btnSearchPdfProduct";
            this.btnSearchPdfProduct.Size = new System.Drawing.Size(75, 23);
            this.btnSearchPdfProduct.TabIndex = 4;
            this.btnSearchPdfProduct.Text = "Search";
            this.btnSearchPdfProduct.UseVisualStyleBackColor = true;
            this.btnSearchPdfProduct.Click += new System.EventHandler(this.btnSearchPdfProductFile_Click);
            // 
            // btnBrowsePdfFile
            // 
            this.btnBrowsePdfFile.Location = new System.Drawing.Point(335, 27);
            this.btnBrowsePdfFile.Name = "btnBrowsePdfFile";
            this.btnBrowsePdfFile.Size = new System.Drawing.Size(75, 23);
            this.btnBrowsePdfFile.TabIndex = 3;
            this.btnBrowsePdfFile.Text = "Browse ...";
            this.btnBrowsePdfFile.UseVisualStyleBackColor = true;
            this.btnBrowsePdfFile.Click += new System.EventHandler(this.btnBrowsePdfFile_Click);
            // 
            // lblSelectFile
            // 
            this.lblSelectFile.AutoSize = true;
            this.lblSelectFile.Location = new System.Drawing.Point(16, 35);
            this.lblSelectFile.Name = "lblSelectFile";
            this.lblSelectFile.Size = new System.Drawing.Size(60, 15);
            this.lblSelectFile.TabIndex = 1;
            this.lblSelectFile.Text = "Select file:";
            // 
            // txtPdfFilePath
            // 
            this.txtPdfFilePath.Location = new System.Drawing.Point(91, 27);
            this.txtPdfFilePath.Name = "txtPdfFilePath";
            this.txtPdfFilePath.Size = new System.Drawing.Size(232, 23);
            this.txtPdfFilePath.TabIndex = 2;
            // 
            // openPdfDialog
            // 
            this.openPdfDialog.Filter = "PDF files|*.pdf";
            this.openPdfDialog.Title = "Open products file";
            // 
            // openExcelDialog
            // 
            this.openExcelDialog.Filter = "Excel files|*.xlsx";
            this.openExcelDialog.Title = "Open products file";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 562);
            this.Controls.Add(this.gbSearchPdf);
            this.Controls.Add(this.txtLogs);
            this.Controls.Add(this.gvProducts);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.gbSearchWeb);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(899, 601);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(899, 601);
            this.Name = "Main";
            this.RightToLeftLayout = true;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CSI Scrapper";
            this.Load += new System.EventHandler(this.Main_Load);
            this.gbSearchWeb.ResumeLayout(false);
            this.gbSearchWeb.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvProducts)).EndInit();
            this.gbSearchPdf.ResumeLayout(false);
            this.gbSearchPdf.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEnterSearchTerm;
        private System.Windows.Forms.TextBox txtSearchTerm;
        private System.Windows.Forms.GroupBox gbSearchWeb;
        private System.Windows.Forms.Button btnSearchProduct;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel tsLblStatus;
        private System.Windows.Forms.DataGridView gvProducts;
        private System.Windows.Forms.TextBox txtLogs;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.GroupBox gbSearchPdf;
        private System.Windows.Forms.Button btnBrowsePdfFile;
        private System.Windows.Forms.Label lblSelectFile;
        private System.Windows.Forms.TextBox txtPdfFilePath;
        private System.Windows.Forms.OpenFileDialog openPdfDialog;
        private System.Windows.Forms.Button btnSearchPdfProduct;
        private System.Windows.Forms.Button btnSearchExcelProduct;
        private System.Windows.Forms.TextBox txtExcelFilePath;
        private System.Windows.Forms.Label lblSelectProduct;
        private System.Windows.Forms.ToolStripButton tsBtnInfo;
        private System.Windows.Forms.OpenFileDialog openExcelDialog;
        private System.Windows.Forms.Button btnBrowseExcelFile;
    }
}

