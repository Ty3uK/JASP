namespace JASP.Forms
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
            this.logoBox = new System.Windows.Forms.PictureBox();
            this.descriptionBox = new System.Windows.Forms.Label();
            this.linkBox = new System.Windows.Forms.LinkLabel();
            this.siteBox = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).BeginInit();
            this.SuspendLayout();
            // 
            // logoBox
            // 
            this.logoBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("logoBox.BackgroundImage")));
            this.logoBox.Location = new System.Drawing.Point(0, 19);
            this.logoBox.Name = "logoBox";
            this.logoBox.Size = new System.Drawing.Size(48, 48);
            this.logoBox.TabIndex = 0;
            this.logoBox.TabStop = false;
            // 
            // descriptionBox
            // 
            this.descriptionBox.AutoSize = true;
            this.descriptionBox.Font = new System.Drawing.Font("Georgia", 9.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.descriptionBox.Location = new System.Drawing.Point(54, 19);
            this.descriptionBox.Name = "descriptionBox";
            this.descriptionBox.Size = new System.Drawing.Size(214, 48);
            this.descriptionBox.TabIndex = 1;
            this.descriptionBox.Text = "JASP is a small preprocessor\r\nfor JASS programming\r\nlanguage.";
            this.descriptionBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkBox
            // 
            this.linkBox.AutoSize = true;
            this.linkBox.Location = new System.Drawing.Point(167, 83);
            this.linkBox.Name = "linkBox";
            this.linkBox.Size = new System.Drawing.Size(99, 13);
            this.linkBox.TabIndex = 2;
            this.linkBox.TabStop = true;
            this.linkBox.Text = "http://www.w3jp.ru";
            // 
            // siteBox
            // 
            this.siteBox.AutoSize = true;
            this.siteBox.Location = new System.Drawing.Point(13, 83);
            this.siteBox.Name = "siteBox";
            this.siteBox.Size = new System.Drawing.Size(148, 13);
            this.siteBox.TabIndex = 3;
            this.siteBox.Text = "Read more on our homepage:";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(282, 105);
            this.Controls.Add(this.siteBox);
            this.Controls.Add(this.linkBox);
            this.Controls.Add(this.descriptionBox);
            this.Controls.Add(this.logoBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "JASP - About";
            ((System.ComponentModel.ISupportInitialize)(this.logoBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox logoBox;
        private System.Windows.Forms.Label descriptionBox;
        private System.Windows.Forms.LinkLabel linkBox;
        private System.Windows.Forms.Label siteBox;
    }
}