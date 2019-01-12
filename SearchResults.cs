using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace PlexWalk
{
    public partial class SearchResults : Form, FormInterface
    {
        bool AbortThreads = false;
        List<Descriptor> searches;
        string query;
        public SearchResults(List<Descriptor> searches, string query)
        {
            InitializeComponent();
            this.searches = searches;
            this.query = query;
            this.Text += " - " + query;
        }

        private void SearchResults_Load(object sender, EventArgs e)
        {
            foreach (Descriptor desc in searches)
            {
                TreeNode n = new TreeNode(desc.serverName) { Tag = desc, Name = "/search?query=" + Uri.EscapeDataString(query) };
                n.Nodes.Add("");
                searchTreeView.Nodes.Add(n);
            }
            foreach (TreeNode n in searchTreeView.Nodes)
                n.Expand();
        }

        delegate void ChangeNodeCallback(object sender, TreeNode src);

        private void ExpandNode(object sender, TreeNode src)
        {
            if (((Control)sender).InvokeRequired)
            {
                ChangeNodeCallback d = new ChangeNodeCallback(ExpandNode);
                this.Invoke(d, new object[] { sender, src });
            }
            else
                PlexUtils.populateSubNodes(src, this);
        }

        delegate void AddNodeCallback(TreeNode Parent, TreeNode Child);

        public void AddNode(TreeNode Parent, TreeNode Child)
        {
            if (searchTreeView.InvokeRequired)
            {
                AddNodeCallback d = new AddNodeCallback(AddNode);
                this.Invoke(d, new object[] { Parent, Child });
            }
            else
                Parent.Nodes.Add(Child);
        }

        private void searchTreeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate(object state)
            {
                if (AbortThreads)
                {
                    int total; int count;
                    int max_total ; int max_count;
                    ThreadPool.GetMaxThreads(out max_count, out max_total);
                    ThreadPool.GetAvailableThreads(out count,out total);
                    if (count == max_count && total == max_total)
                        AbortThreads = false;
                    return;
                }
                object[] array = state as object[];
                TreeNode src = (TreeNode)array[0];
                if (src.FirstNode.Text == null || src.FirstNode.Text == "")
                {
                    ClearNodes(sender, src);
                    try
                    {
                        PlexUtils.populateSubNodes(src, this);
                    }
                    catch
                    {
                    }
                }
            }, new object[] { e.Node });
        }
        private void ClearNodes(object sender, TreeNode src)
        {
            if (((Control)sender).InvokeRequired)
            {
                ChangeNodeCallback d = new ChangeNodeCallback(ClearNodes);
                try
                {
                    this.Invoke(d, new object[] { sender, src });
                } catch { }
            }
            else
                src.Nodes.Clear();
        }

        TreeNode selected;
        private void searchTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            selected = ((TreeView)sender).GetNodeAt(e.Location);
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                searchContextMenu.Show();
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadInfo[] di = PlexUtils.getDownloads(selected,this).Select(x => new DownloadInfo(x.getDownloadURL(), PlexUtils.MakeValidFileName(x.downloadFilename), PlexUtils.MakeValidFileName(x.subdir))).ToArray();
            if (PlexUtils.downloadDialog == null || PlexUtils.downloadDialog.IsDisposed)
                PlexUtils.downloadDialog = new DownloadDialog(di);
            else
                PlexUtils.downloadDialog.enqueue(di);
            PlexUtils.downloadDialog.Show();
        }

        private void copyUrlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(PlexUtils.getDownloadURL(selected));
            MessageBox.Show("URL Copied to clipboard");
        }

        private void copyUrlToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            if (selected == null || selected.Tag == null)
            {
                copyUrlToolStripMenuItem.Visible = false;
                playInVlcToolStripMenuItem.Visible = false;
            }
            else
            {
                copyUrlToolStripMenuItem.Visible = ((Descriptor)selected.Tag).canDownload;
                playInVlcToolStripMenuItem.Visible = ((Descriptor)selected.Tag).canDownload;
            }
        }

        private void playInVlcToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlexUtils.PlayInVLC(selected);
        }

        private void SearchResults_FormClosing(object sender, FormClosingEventArgs e)
        {
            AbortThreads = true;
        }
    }
}
