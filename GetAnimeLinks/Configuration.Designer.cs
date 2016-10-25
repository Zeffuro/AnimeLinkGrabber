namespace GetAnimeLinks
{
    partial class Configuration
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
            this.cbxDefaultPlayer = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cbxDefaultPlayer
            // 
            this.cbxDefaultPlayer.FormattingEnabled = true;
            this.cbxDefaultPlayer.Items.AddRange(new object[] {
            "Media Player Classic",
            "VideoLAN"});
            this.cbxDefaultPlayer.Location = new System.Drawing.Point(53, 12);
            this.cbxDefaultPlayer.Name = "cbxDefaultPlayer";
            this.cbxDefaultPlayer.Size = new System.Drawing.Size(121, 21);
            this.cbxDefaultPlayer.TabIndex = 0;
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 259);
            this.Controls.Add(this.cbxDefaultPlayer);
            this.Name = "Configuration";
            this.Text = "Configuration";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxDefaultPlayer;
    }
}