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
            this.components = new System.ComponentModel.Container();
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
            this.searchTreeView.Location = new System.Drawing.Point(12, 12);
            this.searchTreeView.Name = "searchTreeView";
            this.searchTreeView.Size = new System.Drawing.Size(383, 383);
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 407);
            this.Controls.Add(this.searchTreeView);
            this.Name = "SearchResults";
            this.Text = "SearchResults";
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