namespace RenDownloader
{
    partial class MainFrm
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
            this.lblSaveTo = new System.Windows.Forms.Label();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.btnSelectSaveDir = new System.Windows.Forms.Button();
            this.txtEpisodeUrls = new System.Windows.Forms.TextBox();
            this.lblUrls = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.txtStartDownload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblSaveTo
            // 
            this.lblSaveTo.AutoSize = true;
            this.lblSaveTo.Location = new System.Drawing.Point(20, 11);
            this.lblSaveTo.Name = "lblSaveTo";
            this.lblSaveTo.Size = new System.Drawing.Size(47, 13);
            this.lblSaveTo.TabIndex = 0;
            this.lblSaveTo.Text = "Save to:";
            // 
            // txtSavePath
            // 
            this.txtSavePath.Location = new System.Drawing.Point(17, 27);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.ReadOnly = true;
            this.txtSavePath.Size = new System.Drawing.Size(327, 20);
            this.txtSavePath.TabIndex = 1;
            // 
            // btnSelectSaveDir
            // 
            this.btnSelectSaveDir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSelectSaveDir.Location = new System.Drawing.Point(348, 27);
            this.btnSelectSaveDir.Name = "btnSelectSaveDir";
            this.btnSelectSaveDir.Size = new System.Drawing.Size(33, 20);
            this.btnSelectSaveDir.TabIndex = 2;
            this.btnSelectSaveDir.Text = "...";
            this.btnSelectSaveDir.UseVisualStyleBackColor = true;
            this.btnSelectSaveDir.Click += new System.EventHandler(this.btnSelectSaveDir_Click);
            // 
            // txtEpisodeUrls
            // 
            this.txtEpisodeUrls.Location = new System.Drawing.Point(17, 76);
            this.txtEpisodeUrls.Multiline = true;
            this.txtEpisodeUrls.Name = "txtEpisodeUrls";
            this.txtEpisodeUrls.Size = new System.Drawing.Size(364, 175);
            this.txtEpisodeUrls.TabIndex = 3;
            // 
            // lblUrls
            // 
            this.lblUrls.AutoSize = true;
            this.lblUrls.Location = new System.Drawing.Point(20, 59);
            this.lblUrls.Name = "lblUrls";
            this.lblUrls.Size = new System.Drawing.Size(53, 13);
            this.lblUrls.TabIndex = 4;
            this.lblUrls.Text = "Episodes:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(100, 258);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(281, 23);
            this.progressBar1.TabIndex = 5;
            // 
            // txtStartDownload
            // 
            this.txtStartDownload.Location = new System.Drawing.Point(17, 258);
            this.txtStartDownload.Name = "txtStartDownload";
            this.txtStartDownload.Size = new System.Drawing.Size(75, 23);
            this.txtStartDownload.TabIndex = 6;
            this.txtStartDownload.Text = "Download";
            this.txtStartDownload.UseVisualStyleBackColor = true;
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 298);
            this.Controls.Add(this.txtStartDownload);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.lblUrls);
            this.Controls.Add(this.txtEpisodeUrls);
            this.Controls.Add(this.btnSelectSaveDir);
            this.Controls.Add(this.txtSavePath);
            this.Controls.Add(this.lblSaveTo);
            this.Name = "MainFrm";
            this.Text = "Ren Downloader";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSaveTo;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Button btnSelectSaveDir;
        private System.Windows.Forms.TextBox txtEpisodeUrls;
        private System.Windows.Forms.Label lblUrls;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button txtStartDownload;
    }
}

