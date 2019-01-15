namespace PlexWalk
{
    partial class DownloadDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadDialog));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.DownloadList = new System.Windows.Forms.ListBox();
            this.ExitButton = new System.Windows.Forms.Button();
            this.StartButton = new System.Windows.Forms.Button();
            this.Speed = new System.Windows.Forms.Label();
            this.OverallProgress = new iTalk.iTalk_ProgressBar();
            this.CurrentProgress = new iTalk.iTalk_ProgressBar();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(13, 261);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(361, 21);
            this.label1.TabIndex = 3;
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DownloadList
            // 
            this.DownloadList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DownloadList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(48)))), ((int)(((byte)(52)))));
            this.DownloadList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DownloadList.ContextMenuStrip = this.contextMenuStrip1;
            this.DownloadList.ForeColor = System.Drawing.Color.White;
            this.DownloadList.FormattingEnabled = true;
            this.DownloadList.ItemHeight = 15;
            this.DownloadList.Location = new System.Drawing.Point(0, 1);
            this.DownloadList.Name = "DownloadList";
            this.DownloadList.Size = new System.Drawing.Size(387, 255);
            this.DownloadList.TabIndex = 2;
            // 
            // ExitButton
            // 
            this.ExitButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ExitButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ExitButton.Image = global::PlexWalk.Properties.Resources.exit;
            this.ExitButton.Location = new System.Drawing.Point(283, 399);
            this.ExitButton.Margin = new System.Windows.Forms.Padding(2);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(91, 35);
            this.ExitButton.TabIndex = 10;
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // StartButton
            // 
            this.StartButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.StartButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartButton.Image = global::PlexWalk.Properties.Resources.start;
            this.StartButton.Location = new System.Drawing.Point(19, 399);
            this.StartButton.Margin = new System.Windows.Forms.Padding(2);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(91, 35);
            this.StartButton.TabIndex = 9;
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.Start_Click);
            // 
            // Speed
            // 
            this.Speed.BackColor = System.Drawing.Color.Transparent;
            this.Speed.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Speed.ForeColor = System.Drawing.Color.White;
            this.Speed.Location = new System.Drawing.Point(0, 378);
            this.Speed.Name = "Speed";
            this.Speed.Size = new System.Drawing.Size(387, 19);
            this.Speed.TabIndex = 13;
            this.Speed.Text = "Speed";
            this.Speed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // OverallProgress
            // 
            this.OverallProgress.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.OverallProgress.Location = new System.Drawing.Point(257, 281);
            this.OverallProgress.Maximum = ((long)(100));
            this.OverallProgress.MinimumSize = new System.Drawing.Size(117, 115);
            this.OverallProgress.Name = "OverallProgress";
            this.OverallProgress.ProgressColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.OverallProgress.ProgressColor2 = System.Drawing.Color.Yellow;
            this.OverallProgress.ProgressShape = iTalk.iTalk_ProgressBar._ProgressShape.Round;
            this.OverallProgress.Size = new System.Drawing.Size(117, 117);
            this.OverallProgress.TabIndex = 12;
            this.OverallProgress.Text = "iTalk_ProgressBar1";
            this.OverallProgress.Value = ((long)(0));
            // 
            // CurrentProgress
            // 
            this.CurrentProgress.Font = new System.Drawing.Font("Segoe UI", 15F);
            this.CurrentProgress.Location = new System.Drawing.Point(19, 281);
            this.CurrentProgress.Maximum = ((long)(100));
            this.CurrentProgress.MinimumSize = new System.Drawing.Size(117, 115);
            this.CurrentProgress.Name = "CurrentProgress";
            this.CurrentProgress.ProgressColor1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.CurrentProgress.ProgressColor2 = System.Drawing.Color.Yellow;
            this.CurrentProgress.ProgressShape = iTalk.iTalk_ProgressBar._ProgressShape.Round;
            this.CurrentProgress.Size = new System.Drawing.Size(117, 117);
            this.CurrentProgress.TabIndex = 11;
            this.CurrentProgress.Text = "progressBar3";
            this.CurrentProgress.Value = ((long)(0));
            // 
            // DownloadDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(43)))), ((int)(((byte)(48)))), ((int)(((byte)(52)))));
            this.ClientSize = new System.Drawing.Size(388, 445);
            this.Controls.Add(this.OverallProgress);
            this.Controls.Add(this.CurrentProgress);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.StartButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DownloadList);
            this.Controls.Add(this.Speed);
            this.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Bold);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DownloadDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Download Queue";
            this.Load += new System.EventHandler(this.DownloadDialog_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ListBox DownloadList;
        private iTalk.iTalk_ProgressBar CurrentProgress;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Button StartButton;
        private iTalk.iTalk_ProgressBar OverallProgress;
        private System.Windows.Forms.Label Speed;
    }
}