namespace PlexWalk
{
    partial class SearchResults
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchResults));
            this.searchTreeView = new System.Windows.Forms.TreeView();
            this.searchContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.downloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyUrlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playInVlcToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchTreeView
            // 
            this.searchTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(48)))), ((int)(((byte)(52)))));
            this.searchTreeView.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold);
            this.searchTreeView.ForeColor = System.Drawing.Color.White;
            this.searchTreeView.Location = new System.Drawing.Point(9, 10);
            this.searchTreeView.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.searchTreeView.Name = "searchTreeView";
            this.searchTreeView.Size = new System.Drawing.Size(367, 517);
            this.searchTreeView.TabIndex = 0;
            this.searchTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.searchTreeView_BeforeExpand);
            this.searchTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.searchTreeView_MouseUp);
            // 
            // searchContextMenu
            // 
            this.searchContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.searchContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.downloadToolStripMenuItem,
            this.copyUrlToolStripMenuItem,
            this.playInVlcToolStripMenuItem});
            this.searchContextMenu.Name = "searchContextMenu";
            this.searchContextMenu.Size = new System.Drawing.Size(176, 104);
            // 
            // downloadToolStripMenuItem
            // 
            this.downloadToolStripMenuItem.Name = "downloadToolStripMenuItem";
            this.downloadToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.downloadToolStripMenuItem.Text = "Download";
            this.downloadToolStripMenuItem.Click += new System.EventHandler(this.downloadToolStripMenuItem_Click);
            // 
            // copyUrlToolStripMenuItem
            // 
            this.copyUrlToolStripMenuItem.Name = "copyUrlToolStripMenuItem";
            this.copyUrlToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.copyUrlToolStripMenuItem.Text = "Copy Url";
            this.copyUrlToolStripMenuItem.DropDownOpening += new System.EventHandler(this.copyUrlToolStripMenuItem_DropDownOpening);
            this.copyUrlToolStripMenuItem.Click += new System.EventHandler(this.copyUrlToolStripMenuItem_Click);
            // 
            // playInVlcToolStripMenuItem
            // 
            this.playInVlcToolStripMenuItem.Name = "playInVlcToolStripMenuItem";
            this.playInVlcToolStripMenuItem.Size = new System.Drawing.Size(175, 24);
            this.playInVlcToolStripMenuItem.Text = "Play in VLC";
            this.playInVlcToolStripMenuItem.Click += new System.EventHandler(this.playInVlcToolStripMenuItem_Click);
            // 
            // SearchResults
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(48)))), ((int)(((byte)(52)))));
            this.ClientSize = new System.Drawing.Size(384, 536);
            this.Controls.Add(this.searchTreeView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.Name = "SearchResults";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Search Results";
            this.Load += new System.EventHandler(this.SearchResults_Load);
            this.searchContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView searchTreeView;
        private System.Windows.Forms.ContextMenuStrip searchContextMenu;
        private System.Windows.Forms.ToolStripMenuItem downloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyUrlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playInVlcToolStripMenuItem;

    }
}