namespace PlexWalk
{
    partial class BrowseForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowseForm));
            this.plexTreeView = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playInVLCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.ExpandButton = new System.Windows.Forms.ToolStripButton();
            this.RefreshButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.LogOutButton = new System.Windows.Forms.ToolStripButton();
            this.AnimeButton = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripMenuItem();
            this.SearchButton = new System.Windows.Forms.ToolStripMenuItem();
            this.searchServersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // plexTreeView
            // 
            this.plexTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plexTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(48)))), ((int)(((byte)(52)))));
            this.plexTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.plexTreeView.ContextMenuStrip = this.contextMenuStrip1;
            this.plexTreeView.ForeColor = System.Drawing.Color.White;
            this.plexTreeView.Location = new System.Drawing.Point(12, 29);
            this.plexTreeView.Name = "plexTreeView";
            this.plexTreeView.Size = new System.Drawing.Size(542, 491);
            this.plexTreeView.TabIndex = 0;
            this.plexTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.plexTreeView.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.plexTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.plexTreeView_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.playInVLCToolStripMenuItem,
            this.downloadToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(150, 76);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(149, 24);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.RefreshToolStripMenuItem_Click);
            // 
            // playInVLCToolStripMenuItem
            // 
            this.playInVLCToolStripMenuItem.Name = "playInVLCToolStripMenuItem";
            this.playInVLCToolStripMenuItem.Size = new System.Drawing.Size(149, 24);
            this.playInVLCToolStripMenuItem.Text = "Play in VLC";
            this.playInVLCToolStripMenuItem.Click += new System.EventHandler(this.playInVLCToolStripMenuItem_Click);
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new System.Drawing.Size(149, 24);
            this.downloadToolStripMenuItem.Text = "Download";
            this.downloadToolStripMenuItem.Click += new System.EventHandler(this.downloadToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ExpandButton,
            this.RefreshButton,
            this.toolStripDropDownButton1,
            this.LogOutButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(565, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // ExpandButton
            // 
            this.ExpandButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ExpandButton.Image = ((System.Drawing.Image)(resources.GetObject("ExpandButton.Image")));
            this.ExpandButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ExpandButton.Name = "ExpandButton";
            this.ExpandButton.Size = new System.Drawing.Size(24, 24);
            this.ExpandButton.Text = "toolStripButton1";
            this.ExpandButton.ToolTipText = "Expand all";
            this.ExpandButton.Click += new System.EventHandler(this.ExpandServers_Click);
            // 
            // RefreshButton
            // 
            this.RefreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RefreshButton.Image = global::PlexWalk.Properties.Resources.refresh;
            this.RefreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(24, 24);
            this.RefreshButton.Text = "toolStripButton2";
            this.RefreshButton.ToolTipText = "Refresh";
            this.RefreshButton.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SearchButton,
            this.toolStripButton1,
            this.AnimeButton,
            this.searchServersToolStripMenuItem});
            this.toolStripDropDownButton1.Image = global::PlexWalk.Properties.Resources.search;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(34, 24);
            this.toolStripDropDownButton1.Text = "Search";
            // 
            // LogOutButton
            // 
            this.LogOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.LogOutButton.Image = ((System.Drawing.Image)(resources.GetObject("LogOutButton.Image")));
            this.LogOutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.LogOutButton.Name = "LogOutButton";
            this.LogOutButton.Size = new System.Drawing.Size(66, 24);
            this.LogOutButton.Text = "Log Out";
            this.LogOutButton.Click += new System.EventHandler(this.LogoutButton_Click);
            // 
            // AnimeButton
            // 
            this.AnimeButton.Image = global::PlexWalk.Properties.Resources.YesAnime;
            this.AnimeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AnimeButton.Name = "AnimeButton";
            this.AnimeButton.Size = new System.Drawing.Size(181, 26);
            this.AnimeButton.Text = "Anime!";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::PlexWalk.Properties.Resources.tv;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(181, 26);
            this.toolStripButton1.Text = "Search TV";
            this.toolStripButton1.Click += new System.EventHandler(this.TVSearchButton_Click);
            // 
            // SearchButton
            // 
            this.SearchButton.Image = global::PlexWalk.Properties.Resources.movie_512;
            this.SearchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(181, 26);
            this.SearchButton.Text = "Search Movies";
            // 
            // searchServersToolStripMenuItem
            // 
            this.searchServersToolStripMenuItem.Image = global::PlexWalk.Properties.Resources.search;
            this.searchServersToolStripMenuItem.Name = "searchServersToolStripMenuItem";
            this.searchServersToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.searchServersToolStripMenuItem.Text = "Search Servers";
            this.searchServersToolStripMenuItem.Click += new System.EventHandler(this.searchServersToolStripMenuItem_Click);
            // 
            // BrowseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(48)))), ((int)(((byte)(52)))));
            this.ClientSize = new System.Drawing.Size(565, 532);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.plexTreeView);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BrowseForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Server List";
            this.Load += new System.EventHandler(this.BrowseForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView plexTreeView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton ExpandButton;
        private System.Windows.Forms.ToolStripButton RefreshButton;
        private System.Windows.Forms.ToolStripMenuItem playInVLCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton LogOutButton;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem SearchButton;
        private System.Windows.Forms.ToolStripMenuItem toolStripButton1;
        private System.Windows.Forms.ToolStripMenuItem AnimeButton;
        private System.Windows.Forms.ToolStripMenuItem searchServersToolStripMenuItem;
    }
}

