namespace GetAnimeLinks
{
    partial class Form1
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
            this.cbxSite = new System.Windows.Forms.ComboBox();
            this.lblSite = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.cbxSearchResults = new System.Windows.Forms.ComboBox();
            this.lblFound = new System.Windows.Forms.Label();
            this.cbxEpisodes = new System.Windows.Forms.ComboBox();
            this.btnCopySource = new System.Windows.Forms.Button();
            this.btnPrevEpisode = new System.Windows.Forms.Button();
            this.btnNextEpisode = new System.Windows.Forms.Button();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txtEpisodeLink = new System.Windows.Forms.TextBox();
            this.pbxWait = new System.Windows.Forms.PictureBox();
            this.pcbAnime = new System.Windows.Forms.PictureBox();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.checkOrder = new System.Windows.Forms.CheckBox();
            this.btnMal = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.btnOpenSite = new System.Windows.Forms.Button();
            this.btnCopyAll = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.linksToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.yudofuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.lstRecentlyWatched = new System.Windows.Forms.ListBox();
            this.btnClearRecentlyWatched = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxWait)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbAnime)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbxSite
            // 
            this.cbxSite.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSite.FormattingEnabled = true;
            this.cbxSite.Items.AddRange(new object[] {
            "AnimeDao",
            "AnimeJolt",
            "AnimesTV",
            "GoGoAnime",
            "WatchAnimeOnline",
            "MoeTube",
            "MoeTube (Mobile)"});
            this.cbxSite.Location = new System.Drawing.Point(79, 28);
            this.cbxSite.Name = "cbxSite";
            this.cbxSite.Size = new System.Drawing.Size(161, 21);
            this.cbxSite.TabIndex = 0;
            this.cbxSite.SelectedIndexChanged += new System.EventHandler(this.cbxSite_SelectedIndexChanged);
            // 
            // lblSite
            // 
            this.lblSite.AutoSize = true;
            this.lblSite.Location = new System.Drawing.Point(13, 33);
            this.lblSite.Name = "lblSite";
            this.lblSite.Size = new System.Drawing.Size(60, 13);
            this.lblSite.TabIndex = 1;
            this.lblSite.Text = "Anime Site:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(296, 29);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(161, 20);
            this.txtSearch.TabIndex = 2;
            this.txtSearch.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSearch_KeyPress);
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(246, 31);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.TabIndex = 3;
            this.lblSearch.Text = "Search:";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(463, 27);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(81, 50);
            this.btnSearch.TabIndex = 4;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cbxSearchResults
            // 
            this.cbxSearchResults.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSearchResults.FormattingEnabled = true;
            this.cbxSearchResults.Location = new System.Drawing.Point(79, 56);
            this.cbxSearchResults.Name = "cbxSearchResults";
            this.cbxSearchResults.Size = new System.Drawing.Size(378, 21);
            this.cbxSearchResults.TabIndex = 5;
            this.cbxSearchResults.SelectedIndexChanged += new System.EventHandler(this.cbxSearchResults_SelectedIndexChanged);
            // 
            // lblFound
            // 
            this.lblFound.AutoSize = true;
            this.lblFound.Location = new System.Drawing.Point(13, 58);
            this.lblFound.Name = "lblFound";
            this.lblFound.Size = new System.Drawing.Size(39, 13);
            this.lblFound.TabIndex = 6;
            this.lblFound.Text = "Anime:";
            // 
            // cbxEpisodes
            // 
            this.cbxEpisodes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxEpisodes.FormattingEnabled = true;
            this.cbxEpisodes.Location = new System.Drawing.Point(250, 244);
            this.cbxEpisodes.Name = "cbxEpisodes";
            this.cbxEpisodes.Size = new System.Drawing.Size(264, 21);
            this.cbxEpisodes.TabIndex = 9;
            this.cbxEpisodes.SelectedIndexChanged += new System.EventHandler(this.cbxEpisodes_SelectedIndexChanged);
            // 
            // btnCopySource
            // 
            this.btnCopySource.Location = new System.Drawing.Point(470, 297);
            this.btnCopySource.Name = "btnCopySource";
            this.btnCopySource.Size = new System.Drawing.Size(75, 23);
            this.btnCopySource.TabIndex = 10;
            this.btnCopySource.Text = "Copy";
            this.btnCopySource.UseVisualStyleBackColor = true;
            this.btnCopySource.Click += new System.EventHandler(this.btnCopySource_Click);
            // 
            // btnPrevEpisode
            // 
            this.btnPrevEpisode.Location = new System.Drawing.Point(218, 243);
            this.btnPrevEpisode.Name = "btnPrevEpisode";
            this.btnPrevEpisode.Size = new System.Drawing.Size(24, 23);
            this.btnPrevEpisode.TabIndex = 11;
            this.btnPrevEpisode.Text = "<-";
            this.btnPrevEpisode.UseVisualStyleBackColor = true;
            this.btnPrevEpisode.Click += new System.EventHandler(this.btnPrevEpisode_Click);
            // 
            // btnNextEpisode
            // 
            this.btnNextEpisode.Location = new System.Drawing.Point(520, 243);
            this.btnNextEpisode.Name = "btnNextEpisode";
            this.btnNextEpisode.Size = new System.Drawing.Size(24, 23);
            this.btnNextEpisode.TabIndex = 12;
            this.btnNextEpisode.Text = "->";
            this.btnNextEpisode.UseVisualStyleBackColor = true;
            this.btnNextEpisode.Click += new System.EventHandler(this.btnNextEpisode_Click);
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(218, 83);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(326, 154);
            this.txtDescription.TabIndex = 14;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(308, 320);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtEpisodeLink
            // 
            this.txtEpisodeLink.Location = new System.Drawing.Point(218, 271);
            this.txtEpisodeLink.Name = "txtEpisodeLink";
            this.txtEpisodeLink.ReadOnly = true;
            this.txtEpisodeLink.Size = new System.Drawing.Size(326, 20);
            this.txtEpisodeLink.TabIndex = 16;
            this.txtEpisodeLink.Enter += new System.EventHandler(this.txtEpisodeLink_Enter);
            // 
            // pbxWait
            // 
            this.pbxWait.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxWait.Image = global::GetAnimeLinks.Properties.Resources.ajax_loader;
            this.pbxWait.Location = new System.Drawing.Point(218, 119);
            this.pbxWait.Name = "pbxWait";
            this.pbxWait.Size = new System.Drawing.Size(100, 100);
            this.pbxWait.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbxWait.TabIndex = 17;
            this.pbxWait.TabStop = false;
            this.pbxWait.Visible = false;
            // 
            // pcbAnime
            // 
            this.pcbAnime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pcbAnime.Location = new System.Drawing.Point(12, 83);
            this.pcbAnime.Name = "pcbAnime";
            this.pcbAnime.Size = new System.Drawing.Size(200, 280);
            this.pcbAnime.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pcbAnime.TabIndex = 13;
            this.pcbAnime.TabStop = false;
            // 
            // lblCopyright
            // 
            this.lblCopyright.AutoSize = true;
            this.lblCopyright.Location = new System.Drawing.Point(467, 352);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(77, 13);
            this.lblCopyright.TabIndex = 18;
            this.lblCopyright.Text = "Made by Jeffro";
            // 
            // checkOrder
            // 
            this.checkOrder.AutoSize = true;
            this.checkOrder.Location = new System.Drawing.Point(218, 297);
            this.checkOrder.Name = "checkOrder";
            this.checkOrder.Size = new System.Drawing.Size(95, 17);
            this.checkOrder.TabIndex = 19;
            this.checkOrder.Text = "Reverse Order";
            this.checkOrder.UseVisualStyleBackColor = true;
            this.checkOrder.CheckedChanged += new System.EventHandler(this.checkOrder_CheckedChanged);
            // 
            // btnMal
            // 
            this.btnMal.Location = new System.Drawing.Point(218, 320);
            this.btnMal.Name = "btnMal";
            this.btnMal.Size = new System.Drawing.Size(82, 23);
            this.btnMal.TabIndex = 20;
            this.btnMal.Text = "MyAnimeList";
            this.btnMal.UseVisualStyleBackColor = true;
            this.btnMal.Visible = false;
            this.btnMal.Click += new System.EventHandler(this.btnMal_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.Location = new System.Drawing.Point(389, 297);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(75, 23);
            this.btnPlay.TabIndex = 21;
            this.btnPlay.Text = "Play Locally";
            this.btnPlay.UseVisualStyleBackColor = true;
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnOpenSite
            // 
            this.btnOpenSite.Location = new System.Drawing.Point(389, 326);
            this.btnOpenSite.Name = "btnOpenSite";
            this.btnOpenSite.Size = new System.Drawing.Size(75, 23);
            this.btnOpenSite.TabIndex = 22;
            this.btnOpenSite.Text = "Play on Site";
            this.btnOpenSite.UseVisualStyleBackColor = true;
            this.btnOpenSite.Click += new System.EventHandler(this.btnOpenSite_Click);
            // 
            // btnCopyAll
            // 
            this.btnCopyAll.Location = new System.Drawing.Point(470, 326);
            this.btnCopyAll.Name = "btnCopyAll";
            this.btnCopyAll.Size = new System.Drawing.Size(75, 23);
            this.btnCopyAll.TabIndex = 23;
            this.btnCopyAll.Text = "Copy All Links";
            this.btnCopyAll.UseVisualStyleBackColor = true;
            this.btnCopyAll.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.linksToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(906, 24);
            this.menuStrip1.TabIndex = 24;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configurationToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // configurationToolStripMenuItem
            // 
            this.configurationToolStripMenuItem.Name = "configurationToolStripMenuItem";
            this.configurationToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.configurationToolStripMenuItem.Text = "Configuration";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // linksToolStripMenuItem
            // 
            this.linksToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.yudofuToolStripMenuItem});
            this.linksToolStripMenuItem.Name = "linksToolStripMenuItem";
            this.linksToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.linksToolStripMenuItem.Text = "Links";
            // 
            // yudofuToolStripMenuItem
            // 
            this.yudofuToolStripMenuItem.Name = "yudofuToolStripMenuItem";
            this.yudofuToolStripMenuItem.Size = new System.Drawing.Size(351, 22);
            this.yudofuToolStripMenuItem.Text = "Yudofu - Watch videos with friends at the same time";
            this.yudofuToolStripMenuItem.Click += new System.EventHandler(this.yudofuToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem1});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem1
            // 
            this.aboutToolStripMenuItem1.Name = "aboutToolStripMenuItem1";
            this.aboutToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
            this.aboutToolStripMenuItem1.Text = "About";
            this.aboutToolStripMenuItem1.Click += new System.EventHandler(this.aboutToolStripMenuItem1_Click);
            // 
            // lstRecentlyWatched
            // 
            this.lstRecentlyWatched.FormattingEnabled = true;
            this.lstRecentlyWatched.Location = new System.Drawing.Point(550, 28);
            this.lstRecentlyWatched.Name = "lstRecentlyWatched";
            this.lstRecentlyWatched.Size = new System.Drawing.Size(344, 290);
            this.lstRecentlyWatched.TabIndex = 25;
            this.lstRecentlyWatched.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstRecentlyWatched_MouseDoubleClick);
            // 
            // btnClearRecentlyWatched
            // 
            this.btnClearRecentlyWatched.Location = new System.Drawing.Point(819, 326);
            this.btnClearRecentlyWatched.Name = "btnClearRecentlyWatched";
            this.btnClearRecentlyWatched.Size = new System.Drawing.Size(75, 23);
            this.btnClearRecentlyWatched.TabIndex = 26;
            this.btnClearRecentlyWatched.Text = "Clear";
            this.btnClearRecentlyWatched.UseVisualStyleBackColor = true;
            this.btnClearRecentlyWatched.Click += new System.EventHandler(this.btnClearRecentlyWatched_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 375);
            this.Controls.Add(this.btnClearRecentlyWatched);
            this.Controls.Add(this.lstRecentlyWatched);
            this.Controls.Add(this.btnCopyAll);
            this.Controls.Add(this.btnOpenSite);
            this.Controls.Add(this.btnPlay);
            this.Controls.Add(this.btnMal);
            this.Controls.Add(this.checkOrder);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.pbxWait);
            this.Controls.Add(this.txtEpisodeLink);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.pcbAnime);
            this.Controls.Add(this.btnNextEpisode);
            this.Controls.Add(this.btnPrevEpisode);
            this.Controls.Add(this.btnCopySource);
            this.Controls.Add(this.cbxEpisodes);
            this.Controls.Add(this.lblFound);
            this.Controls.Add(this.cbxSearchResults);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.lblSite);
            this.Controls.Add(this.cbxSite);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Jeffro\'s AnimeLink Grabber";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxWait)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcbAnime)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbxSite;
        private System.Windows.Forms.Label lblSite;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cbxSearchResults;
        private System.Windows.Forms.Label lblFound;
        private System.Windows.Forms.ComboBox cbxEpisodes;
        private System.Windows.Forms.Button btnCopySource;
        private System.Windows.Forms.Button btnPrevEpisode;
        private System.Windows.Forms.Button btnNextEpisode;
        private System.Windows.Forms.PictureBox pcbAnime;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtEpisodeLink;
        private System.Windows.Forms.PictureBox pbxWait;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.CheckBox checkOrder;
        private System.Windows.Forms.Button btnMal;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnOpenSite;
        private System.Windows.Forms.Button btnCopyAll;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem configurationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem linksToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem yudofuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem1;
        private System.Windows.Forms.ListBox lstRecentlyWatched;
        private System.Windows.Forms.Button btnClearRecentlyWatched;
    }
}

