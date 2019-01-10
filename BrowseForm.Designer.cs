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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.Expand = new System.Windows.Forms.ToolStripButton();
            this.Refresh = new System.Windows.Forms.ToolStripButton();
            this.Search = new System.Windows.Forms.ToolStripButton();
            this.playInVLCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.contextMenuStrip1.Size = new System.Drawing.Size(176, 104);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.RefreshToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Expand,
            this.Refresh,
            this.Search});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(565, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // Expand
            // 
            this.Expand.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Expand.Image = ((System.Drawing.Image)(resources.GetObject("Expand.Image")));
            this.Expand.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Expand.Name = "Expand";
            this.Expand.Size = new System.Drawing.Size(24, 24);
            this.Expand.Text = "toolStripButton1";
            this.Expand.ToolTipText = "Expand all";
            this.Expand.Click += new System.EventHandler(this.ExpandServers_Click);
            // 
            // Refresh
            // 
            this.Refresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Refresh.Image = global::PlexWalk.Properties.Resources.refresh;
            this.Refresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Refresh.Name = "Refresh";
            this.Refresh.Size = new System.Drawing.Size(24, 24);
            this.Refresh.Text = "toolStripButton2";
            this.Refresh.ToolTipText = "Refresh";
            this.Refresh.Click += new System.EventHandler(this.Refresh_Click);
            // 
            // Search
            // 
            this.Search.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.Search.Image = global::PlexWalk.Properties.Resources.search;
            this.Search.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Search.Name = "Search";
            this.Search.Size = new System.Drawing.Size(24, 24);
            this.Search.Text = "toolStripButton3";
            this.Search.Click += new System.EventHandler(this.Search_Click_1);
            // 
            // playInVLCToolStripMenuItem
            // 
            this.playInVLCToolStripMenuItem.Name = "playInVLCToolStripMenuItem";
            this.playInVLCToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.playInVLCToolStripMenuItem.Text = "Play in VLC";
            this.playInVLCToolStripMenuItem.Click += new System.EventHandler(this.playInVLCToolStripMenuItem_Click);
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.downloadToolStripMenuItem.Text = "Download";
            this.downloadToolStripMenuItem.Click += new System.EventHandler(this.downloadToolStripMenuItem_Click);
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
        private System.Windows.Forms.ToolStripButton Expand;
        private System.Windows.Forms.ToolStripButton Refresh;
        private System.Windows.Forms.ToolStripButton Search;
        private System.Windows.Forms.ToolStripMenuItem playInVLCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
    }
}

