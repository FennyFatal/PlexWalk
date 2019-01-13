using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Compression;
using System.IO;
using System.Data.Common;

namespace PlexWalk
{
    public partial class SearchResults : Form, FormInterface
    {
        string title;
        int count = 0;
        bool AbortThreads = false;
        List<Descriptor> searches;
        List<TreeNode> results = new List<TreeNode>();
        string query;
        bool use_db;
        public SearchResults(List<Descriptor> searches, string query, bool use_db = false)
        {
            InitializeComponent();
            this.searches = searches;
            this.query = query;
            title = this.Text + " - " + query;
            this.Text = title + " (" + count + "/" + searches.Count + ")";
            this.use_db = use_db;
        }

        private void SearchResults_Load(object sender, EventArgs e)
        {
            string db_file = "db.db";
            if (use_db 
                && Descriptor.sourceXmlUrl != null 
                && Descriptor.sourceXmlUrl.ToLower().Contains("binary")
                && PlexUtils.LoadDBResources(db_file))
            {
                using (DbConnection cnn = System.Data.SQLite.SQLiteFactory.Instance.CreateConnection())
                {
                    cnn.ConnectionString = "Data Source=" + db_file;
                    cnn.Open();
                    var command = cnn.CreateCommand();
                    command.CommandText = String.Format("Select * from vMovies where title like '%{0}%'", query.Replace("'", "''"));
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string[] things = new string[3];
                        int count = reader.GetValues(things);
                        string dl_fn = '/' + things[1] + things[2].Substring( 
                            things[2].LastIndexOf('.')
                            ,things[2].LastIndexOf('?') - things[2].LastIndexOf('.')
                        );
                        TreeNode n = new TreeNode(things[1]) { 
                            Tag = new Descriptor(
                                new Uri(things[2]).Host
                                , things[2].Substring(things[2].LastIndexOf('=') + 1)
                            ) { canDownload = true
                                , downloadUrl = things[2]
                                , downloadFilename = dl_fn
                                , downloadFullpath = dl_fn
                            } 
                        };
                        searchTreeView.Nodes.Add(n);
                    }
                }
                return;
            }
            var tn = new TreeNode();
            foreach (Descriptor desc in searches)
            {
                tn.Nodes.Add(new TreeNode(desc.serverName) { Tag = desc, Name = "/search?query=" + Uri.EscapeDataString(query) });
            }
            foreach (TreeNode n in tn.Nodes)
                ThreadPool.QueueUserWorkItem(delegate(object state)
                {
                    if (AbortThreads)
                    {
                        int total; int count;
                        int max_total; int max_count;
                        ThreadPool.GetMaxThreads(out max_count, out max_total);
                        ThreadPool.GetAvailableThreads(out count, out total);
                        if (count == max_count && total == max_total)
                            AbortThreads = false;
                        return;
                    }
                    try
                    {
                        PlexUtils.populateSubNodes(n, this, fakeNode);
                        
                    }
                    catch { }
                    UpdateSearchStatus();
                });
        }

        delegate void AnyDelegate();

        private void UpdateSearchStatus()
        {
            if (!this.InvokeRequired)
            {
                this.Text = title + " (" + count++ + "/" + searches.Count + ")";
            }
            else
            {
                try
                {
                    this.Invoke(new AnyDelegate(UpdateSearchStatus));
                }
                catch { }
            }
        }

        private void waitForExit()
        {
            int total; int count; int max_total; int max_count;
            while (!AbortThreads)
            {
                Thread.Sleep(1);
                Application.DoEvents();
                ThreadPool.GetMaxThreads(out max_count, out max_total);
                ThreadPool.GetAvailableThreads(out count, out total);
                if (count == max_count && total == max_total)
                    break;
            }
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
            {
                if (Parent != fakeNode)
                {
                    Parent.Nodes.Add(Child);
                }
                else
                {
                    Thread.Sleep(1);
                    Application.DoEvents();
                    searchTreeView.Nodes.Add(Child);
                }
            }
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

        private TreeNode fakeNode = new TreeNode();

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
            if (((Descriptor)selected.Tag).isSubtitlesNode)
                PlexUtils.PlayInVLC(selected.Parent.PrevNode, selected);
            else
                PlexUtils.PlayInVLC(selected);
        }

        private void SearchResults_FormClosing(object sender, FormClosingEventArgs e)
        {
            AbortThreads = true;
        }
    }
}
