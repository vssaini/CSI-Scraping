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
            this.btnSearch = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsLblStatus = new System.Windows.Forms.ToolStripLabel();
            this.gvProducts = new System.Windows.Forms.DataGridView();
            this.txtLogs = new System.Windows.Forms.TextBox();
            this.bgWebWorker = new System.ComponentModel.BackgroundWorker();
            this.gbSearchWeb.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvProducts)).BeginInit();
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
            this.gbSearchWeb.Controls.Add(this.btnSearch);
            this.gbSearchWeb.Controls.Add(this.lblEnterSearchTerm);
            this.gbSearchWeb.Controls.Add(this.txtSearchTerm);
            this.gbSearchWeb.Location = new System.Drawing.Point(12, 12);
            this.gbSearchWeb.Name = "gbSearchWeb";
            this.gbSearchWeb.Size = new System.Drawing.Size(518, 69);
            this.gbSearchWeb.TabIndex = 3;
            this.gbSearchWeb.TabStop = false;
            this.gbSearchWeb.Text = "Search web for product price";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(425, 26);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 3;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLblStatus});
            this.toolStrip1.Location = new System.Drawing.Point(0, 371);
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
            this.gvProducts.Location = new System.Drawing.Point(12, 97);
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
            this.txtLogs.Size = new System.Drawing.Size(319, 335);
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
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(883, 396);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEnterSearchTerm;
        private System.Windows.Forms.TextBox txtSearchTerm;
        private System.Windows.Forms.GroupBox gbSearchWeb;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel tsLblStatus;
        private System.Windows.Forms.DataGridView gvProducts;
        private System.Windows.Forms.TextBox txtLogs;
        private System.ComponentModel.BackgroundWorker bgWebWorker;
    }
}

