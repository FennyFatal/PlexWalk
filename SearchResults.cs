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
using System.Json;

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

        SearchType searchType = SearchType.SearchServers;

        public enum SearchType
        {
            Library,
            Movie,
            JsonSearchTestCase,
            Shows,
            SearchServers
        }

        public SearchResults(SearchType s, string query)
        {
            InitializeComponent();
            searchType = s;
            this.query = query;
            use_db = false;
        }

        public SearchResults(List<Descriptor> searches, string query, bool use_db = false)
        {
            InitializeComponent();
            this.searches = searches;
            this.query = query;
            title = this.Text + " - " + query;
            this.use_db = use_db;
        }

        private void SearchResults_Load(object sender, EventArgs e)
        {
            if (searchType != SearchType.SearchServers)
            {
                    ThreadPool.QueueUserWorkItem(delegate(object state)
                    {
                        string result = null;
                        using (WebClient wc = new WebClient())
                        {
                            result = wc.DownloadString("http://binaryoutlook.com:8080/?search=" + Uri.EscapeDataString(query) 
                                + (searchType == SearchType.Movie ? "&Movies=true" : "") 
                                + (searchType == SearchType.Library ? "&Sections=true" : "")
                                + (searchType == SearchType.Shows ? "&Shows=true" : "")
                                );
                        }
                        parseJsonResponse(result);
                    });
            }
            else
            {
                this.Text = title + " (" + 0 + "/" + searches.Count + ")";
                var tn = new TreeNode();
                foreach (Descriptor desc in searches)
                {
                    tn.Nodes.Add(new TreeNode(desc.serverName) { Tag = desc, Name = "/search?query=" + Uri.EscapeDataString(query) });
                }
                foreach (TreeNode n in tn.Nodes)
                    ThreadPool.QueueUserWorkItem(delegate (object state)
                    {
                        if (AbortThreads)
                        {
                            int max_count, max_total, count, total;
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
        }

        private void parseJsonResponse(string result)
        {
            var value = System.Json.JsonValue.Parse(result);
            try
            {
                #region Directory
                foreach (var dir in value["Directory"])
                {
                    if (AbortThreads)
                        return;
                    string basePath, token;
                    tryReadServerInfo(dir.Value, out basePath, out token);

                    if (basePath == null || token == null)
                        continue;

                    string title = (string)dir.Value.ValueOrDefault("_title").ReadAs(typeof(String), null);
                    string type = (string)dir.Value.ValueOrDefault("_type").ReadAs(typeof(String), null);
                    string keyVal = (string)dir.Value.ValueOrDefault("_key").ReadAs(typeof(String), null);
                    string serverName = (string)dir.Value.ValueOrDefault("_sourceTitle").ReadAs(typeof(String), null);
                    string librarySectionTitle = (string)dir.Value.ValueOrDefault("_librarySectionTitle").ReadAs(typeof(String), null);
                    string librarySectionID = (string)dir.Value.ValueOrDefault("_librarySectionID").ReadAs(typeof(String), null);
                    TreeNode n = new TreeNode(title)
                    {
                        Tag = new Descriptor(basePath, token)
                        {
                        },
                        Name = keyVal
                    };
                    n.Nodes.Add("");
                    AddNode(fakeNode, n);
                }
                #endregion
            } catch { }
            try
            {
                #region Video
                foreach (var video in value["Video"])
                {
                    if (AbortThreads)
                        return;
                    string basePath, token;
                    tryReadServerInfo(video.Value, out basePath, out token);
                    if (basePath == null || token == null)
                        continue;

                    string title = (string)video.Value.ValueOrDefault("_title").ReadAs(typeof(String), null);
                    string type = (string)video.Value.ValueOrDefault("_type").ReadAs(typeof(String), null);
                    string duration = (string)video.Value.ValueOrDefault("_duration").ReadAs(typeof(String), null);
                    string keyVal = (string)video.Value.ValueOrDefault("_key").ReadAs(typeof(String), null);
                    string serverTitle = (string)video.Value.ValueOrDefault("_sourceTitle").ReadAs(typeof(String), null);
                    foreach (var media in video.Value["Media"])
                    {
                        string width = (string)media.Value.ValueOrDefault("_width").ReadAs(typeof(String), null);
                        string height = (string)media.Value.ValueOrDefault("_height").ReadAs(typeof(String), null);
                        string container = (string)media.Value.ValueOrDefault("_container").ReadAs(typeof(String), null);
                        duration = (string)media.Value.ValueOrDefault("_duration").ReadAs(typeof(String), null);
                        foreach (var part in media.Value["Part"])
                        {
                            string size = (string)part.Value.ValueOrDefault("_size").ReadAs(typeof(String), null);
                            duration = (string)part.Value.ValueOrDefault("_duration").ReadAs(typeof(String), null);
                            string pKey = (string)part.Value.ValueOrDefault("_key").ReadAs(typeof(String), null);
                            string file = (string)part.Value.ValueOrDefault("_file").ReadAs(typeof(String), null);
                            duration = duration == null ? "" : String.Format(" ({0})", TimeSpan.FromMilliseconds(Double.Parse(duration)).ToString(@"hh\:mm\:ss"));
                            TreeNode node = new TreeNode(title + (duration == null ? "" : duration))
                            {
                                Tag = new Descriptor(basePath, token)
                                {
                                    canDownload = false
                                },
                                Name = keyVal
                            };
                            string EndPart = file.Substring(file.LastIndexOf(@"\") + 1).Substring(file.LastIndexOf(@"/") + 1);
                            TreeNode subNode = new TreeNode(String.Format("Download {0}", EndPart))
                            {
                                Tag = new Descriptor((Descriptor)node.Tag)
                                {
                                    canDownload = true,
                                    downloadUrl = string.Format("{0}{1}?{2}"
                                    , basePath
                                    , pKey.Replace("file"
                                    , Uri.EscapeDataString(EndPart.Remove(EndPart.LastIndexOf('.')))), token)
                                },
                                Name = pKey
                            };
                            node.Nodes.Add(subNode);
                            AddNode(fakeNode, node);
                        }
                    }
                }
                #endregion
            } catch { }
        }

        private void tryReadServerInfo(JsonValue value, out string basePath, out string token)
        {
            basePath = null;
            token = null;
            var server = value.ValueOrDefault("Server");
            try
            {
                basePath = String.Format("{0}://{1}:{2}",
                    server["_scheme"].ReadAs(typeof(String)),
                    server["_address"].ReadAs(typeof(String)),
                    server["_port"].ReadAs(typeof(String))
                    );
                token = String.Format("X-Plex-Token={0}", (string)server["_token"].ReadAs(typeof(String)));
            }
            catch { }
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
