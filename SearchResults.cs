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
        bool AbortThreads = false;
        List<Descriptor> searches;
        string query;
        private static bool GetDB = false;
        public SearchResults(List<Descriptor> searches, string query)
        {
            InitializeComponent();
            this.searches = searches;
            this.query = query;
            this.Text += " - " + query;
        }

        private void SearchResults_Load(object sender, EventArgs e)
        {
            if (Descriptor.sourceXmlUrl != null && Descriptor.sourceXmlUrl.ToLower().Contains("binary"))
            {
                string sqliteDllFile = "SQLite.Interop.dll";
                if (!File.Exists(sqliteDllFile))
                {
                    if (Environment.Is64BitProcess)
                    {
                        PlexUtils.ExtractFile(Properties.Resources.SQLite_Interop_dll_x64,sqliteDllFile);
                    }
                    else
                    {
                        PlexUtils.ExtractFile(Properties.Resources.SQLite_Interop_dll_x86, sqliteDllFile);
                    }
                }
                if (!GetDB)
                {
                    string file = "db.db";
                    if (!File.Exists(file))
                    {
                        using (WebClient wc = new WebClient())
                        {
                            var zipData = wc.DownloadData(Descriptor.sourceXmlUrl.Replace(Descriptor.sourceXmlUrl.Substring(Descriptor.sourceXmlUrl.LastIndexOf('/')+1),"db.gz"));
                            PlexUtils.ExtractFile(zipData, file);
                        }
                    }
                    GetDB = true;
                }
                using (DbConnection cnn = System.Data.SQLite.SQLiteFactory.Instance.CreateConnection())
                {
                    cnn.ConnectionString = "Data Source=db.db";
                    cnn.Open();
                    var command = cnn.CreateCommand();
                    command.CommandText = String.Format("Select * from vMovies where title like '%{0}%'", query.Replace("'","''"));
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string[] things = new string[3];
                        int count = reader.GetValues(things);
                        TreeNode n = new TreeNode(things[1]) { Tag = new Descriptor(new Uri(things[2]).Host,things[2].Substring(things[2].LastIndexOf('=')+1)) { canDownload = true, downloadUrl = things[2] }};
                        searchTreeView.Nodes.Add(n);
                    }
                }
                return;
            }
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
