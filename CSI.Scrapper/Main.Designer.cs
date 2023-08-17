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
            this.btnSearchWeb = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsLblStatus = new System.Windows.Forms.ToolStripLabel();
            this.gvProducts = new System.Windows.Forms.DataGridView();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.bgWebWorker = new System.ComponentModel.BackgroundWorker();
            this.gbSearchPdf = new System.Windows.Forms.GroupBox();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.lblSelectFile = new System.Windows.Forms.Label();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnSearchFile = new System.Windows.Forms.Button();
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
            this.gbSearchWeb.Controls.Add(this.btnSearchWeb);
            this.gbSearchWeb.Controls.Add(this.lblEnterSearchTerm);
            this.gbSearchWeb.Controls.Add(this.txtSearchTerm);
            this.gbSearchWeb.Location = new System.Drawing.Point(12, 12);
            this.gbSearchWeb.Name = "gbSearchWeb";
            this.gbSearchWeb.Size = new System.Drawing.Size(518, 69);
            this.gbSearchWeb.TabIndex = 3;
            this.gbSearchWeb.TabStop = false;
            this.gbSearchWeb.Text = "Search web for product price";
            // 
            // btnSearchWeb
            // 
            this.btnSearchWeb.Location = new System.Drawing.Point(425, 26);
            this.btnSearchWeb.Name = "btnSearchWeb";
            this.btnSearchWeb.Size = new System.Drawing.Size(75, 23);
            this.btnSearchWeb.TabIndex = 3;
            this.btnSearchWeb.Text = "Search";
            this.btnSearchWeb.UseVisualStyleBackColor = true;
            this.btnSearchWeb.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLblStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 465);
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
            // gvProducts
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightCyan;
            this.gvProducts.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gvProducts.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gvProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvProducts.Location = new System.Drawing.Point(12, 187);
            this.gvProducts.Name = "gvProducts";
            this.gvProducts.ReadOnly = true;
            this.gvProducts.Size = new System.Drawing.Size(518, 259);
            this.gvProducts.TabIndex = 5;
            // 
            // txtLogs
            // 
            this.txtLogs.BackColor = System.Drawing.SystemColors.Info;
            this.txtLogs.Location = new System.Drawing.Point(549, 21);
            this.txtLogs.Multiline = true;
            this.txtLogs.Name = "txtLogs";
            this.txtLogs.ReadOnly = true;
            this.txtLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogs.Size = new System.Drawing.Size(319, 425);
            this.txtLogs.TabIndex = 6;
            this.txtLogs.Text = "Welcome to CSI Scrapper!";
            // 
            // bgWebWorker
            // 
            this.bgWebWorker.WorkerReportsProgress = true;
            this.bgWebWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWebWorker_DoWork);
            this.bgWebWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWebWorker_ProgressChanged);
            this.bgWebWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWebWorker_RunWorkerCompleted);
            // 
            // gbSearchPdf
            // 
            this.gbSearchPdf.Controls.Add(this.btnSearchFile);
            this.gbSearchPdf.Controls.Add(this.btnBrowseFile);
            this.gbSearchPdf.Controls.Add(this.lblSelectFile);
            this.gbSearchPdf.Controls.Add(this.txtFilePath);
            this.gbSearchPdf.Location = new System.Drawing.Point(12, 98);
            this.gbSearchPdf.Name = "gbSearchPdf";
            this.gbSearchPdf.Size = new System.Drawing.Size(518, 69);
            this.gbSearchPdf.TabIndex = 4;
            this.gbSearchPdf.TabStop = false;
            this.gbSearchPdf.Text = "Search file for product price";
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.Location = new System.Drawing.Point(335, 27);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseFile.TabIndex = 3;
            this.btnBrowseFile.Text = "Browse ...";
            this.btnBrowseFile.UseVisualStyleBackColor = true;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowseFile_Click);
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
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(91, 27);
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(232, 23);
            this.txtFilePath.TabIndex = 2;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "PDF files|*.pdf";
            // 
            // btnSearchFile
            // 
            this.btnSearchFile.Enabled = false;
            this.btnSearchFile.Location = new System.Drawing.Point(425, 26);
            this.btnSearchFile.Name = "btnSearchFile";
            this.btnSearchFile.Size = new System.Drawing.Size(75, 23);
            this.btnSearchFile.TabIndex = 4;
            this.btnSearchFile.Text = "Search";
            this.btnSearchFile.UseVisualStyleBackColor = true;
            this.btnSearchFile.Click += new System.EventHandler(this.btnSearchFile_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 490);
            this.Controls.Add(this.gbSearchPdf);
            this.Controls.Add(this.txtLogs);
            this.Controls.Add(this.gvProducts);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.gbSearchWeb);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.RightToLeftLayout = true;
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
        private System.Windows.Forms.Button btnSearchWeb;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel tsLblStatus;
        private System.Windows.Forms.DataGridView gvProducts;
        private System.Windows.Forms.TextBox txtLogs;
        private System.ComponentModel.BackgroundWorker bgWebWorker;
        private System.Windows.Forms.GroupBox gbSearchPdf;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.Label lblSelectFile;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnSearchFile;
    }
}

