namespace CSI.Scrapper
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.lblWarning = new System.Windows.Forms.Label();
            this.lblUnauthorized = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLblWebsite = new System.Windows.Forms.LinkLabel();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // lblWarning
            // 
            this.lblWarning.Location = new System.Drawing.Point(34, 229);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(299, 38);
            this.lblWarning.TabIndex = 0;
            this.lblWarning.Text = "Warning: This computer program is protected by copyright law and international tr" +
    "eaties.";
            // 
            // lblUnauthorized
            // 
            this.lblUnauthorized.Location = new System.Drawing.Point(34, 287);
            this.lblUnauthorized.Name = "lblUnauthorized";
            this.lblUnauthorized.Size = new System.Drawing.Size(270, 80);
            this.lblUnauthorized.TabIndex = 1;
            this.lblUnauthorized.Text = "Unauthorized reproduction or distribution of this program or any portion of it, m" +
    "ay result in severe civil and criminal penalties, and will be prosecuted to the " +
    "maximum extent possible under the law.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 30);
            this.label1.TabIndex = 2;
            this.label1.Text = "© 2023 Everconnect DS.\r\n       All rights reserved";
            // 
            // linkLblWebsite
            // 
            this.linkLblWebsite.AutoSize = true;
            this.linkLblWebsite.Location = new System.Drawing.Point(87, 191);
            this.linkLblWebsite.Name = "linkLblWebsite";
            this.linkLblWebsite.Size = new System.Drawing.Size(141, 15);
            this.linkLblWebsite.TabIndex = 3;
            this.linkLblWebsite.TabStop = true;
            this.linkLblWebsite.Text = "www.everconnectds.com";
            this.linkLblWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLblWebsite_LinkClicked);
            // 
            // pbLogo
            // 
            this.pbLogo.ErrorImage = null;
            this.pbLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbLogo.Image")));
            this.pbLogo.Location = new System.Drawing.Point(60, 21);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Size = new System.Drawing.Size(198, 97);
            this.pbLogo.TabIndex = 4;
            this.pbLogo.TabStop = false;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 378);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.linkLblWebsite);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblUnauthorized);
            this.Controls.Add(this.lblWarning);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(340, 417);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(340, 417);
            this.Name = "About";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About CSI Scrapper";
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Label lblUnauthorized;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel linkLblWebsite;
        private System.Windows.Forms.PictureBox pbLogo;
    }
}