
namespace YTSpectrum
{
    partial class UI
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
            this.PB_Screen = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.PB_Screen)).BeginInit();
            this.SuspendLayout();
            // 
            // PB_Screen
            // 
            this.PB_Screen.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PB_Screen.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PB_Screen.Location = new System.Drawing.Point(12, 12);
            this.PB_Screen.Name = "PB_Screen";
            this.PB_Screen.Size = new System.Drawing.Size(832, 624);
            this.PB_Screen.TabIndex = 0;
            this.PB_Screen.TabStop = false;
            // 
            // UI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 652);
            this.Controls.Add(this.PB_Screen);
            this.Name = "UI";
            this.Text = "YT Spectrum";
            this.Shown += new System.EventHandler(this.UI_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.PB_Screen)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PB_Screen;
    }
}

