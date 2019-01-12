using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Collections;
using System.Threading;
using Microsoft.Win32;
using System.Diagnostics;
using PlexWalk;

namespace PlexWalk
{
    public partial class BrowseForm : Form, PlexWalk.RootFormInterface
    {
        private RefreshMethod method;
        private DownloadDialog downloadDialog = null;
        static TreeNode selected = null;

        public BrowseForm(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
                parseArgs(args);
        }
        
        public void CloseForm()
        {
            if (plexTreeView.InvokeRequired)
                this.Invoke(new GenericCallback(CloseForm));
            else
                this.Close();
        }

        public RefreshMethod GetRefreshMethod()
        {
            return this.method;
        }

        public RefreshMethod SetRefreshMethod(RefreshMethod method)
        {
            return this.method = method;
        }

        public Dictionary<string, string> GetLaunchArgs()
        {
            return LaunchArgs;
        }

        public Dictionary<string, string> LaunchArgs = new Dictionary<string, string>();

        private void parseArgs(string[] args)
        {
            string last = null;
            foreach (string next in args)
            {
                if (last == null)
                {
                    if (next.StartsWith("-"))
                    {
                        if (next.Contains('='))
                        {
                            if (next.IndexOf('=') + 1 != next.Length)
                            {
                                this.LaunchArgs.Add(
                                next.Substring(0, next.IndexOf('=')).TrimStart('-').ToLower(),
                                next.Substring(next.IndexOf('=') + 1, next.Length - (next.IndexOf('=')) - 1)
                                );
                                last = null;
                            }
                            else
                            {
                                last = next.Trim("-=".ToArray<char>());
                            }
                        }
                        else
                        {
                            last = next.TrimStart('-');
                        }
                    }
                    else
                    {
                        throw new ArgumentException(String.Format("Unknown argument '{0}'", next));
                    }
                }
                else
                {
                    this.LaunchArgs.Add(last, next);
                    last = null;
                }
            }
            if (last != null)
                throw new ArgumentException(String.Format("Required argument missing for '{0}'", last));
        }
        private void BrowseForm_Load(object sender, EventArgs e)
        {

            string parseME = null;

            Descriptor.GUID = Guid.NewGuid().ToString();

            if (LaunchArgs.ContainsKey("owned_path"))
                Descriptor.libraryOwnedPath = LaunchArgs["owned_path"];

            if (LaunchArgs.ContainsKey("server_xml"))
                parseME = PlexUtils.doServerXmlLogin(LaunchArgs["server_xml"].Replace("\"", ""),this);
            else if (LaunchArgs.ContainsKey("token"))
                parseME = PlexUtils.doTokenLogin(LaunchArgs["token"],this);

            if (parseME == null)
                parseME = PlexUtils.doMetaLogin(this);

            loadServerNodesFromXML(parseME);
        }

        public void loadServerNodesFromXML(string parseME)
        {
            plexTreeView.Nodes.Clear();
            foreach (TreeNode tn in PlexUtils.parseServers(parseME))
            {
                plexTreeView.Nodes.Add(tn);
                try
                {
                    //populateSubNodes(tn);
                    tn.Nodes.Add(new TreeNode());
                }
                catch { }
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                object[] array = state as object[];
                TreeNode src = (TreeNode)array[0];
                if (src.FirstNode.Text == null || src.FirstNode.Text == "")
                {
                    ClearNodes(sender, src);
                    try
                    {
                        PlexUtils.populateSubNodes(src,this);
                    }
                    catch
                    {
                    }
                }
            }, new object[] { e.Node });
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

        private void ClearNodes(object sender, TreeNode src)
        {
            if (((Control)sender).InvokeRequired)
            {
                ChangeNodeCallback d = new ChangeNodeCallback(ClearNodes);
                this.Invoke(d, new object[] { sender, src });
            }
            else
                src.Nodes.Clear();
        }

        delegate void GenericCallback();

        delegate void AddNodeCallback(TreeNode Parent, TreeNode Child);

        public void AddNode(TreeNode Parent, TreeNode Child)
        {
            if (plexTreeView.InvokeRequired)
            {
                AddNodeCallback d = new AddNodeCallback(AddNode);
                this.Invoke(d, new object[] { Parent, Child });
            }
            else
                Parent.Nodes.Add(Child);
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (((Descriptor)plexTreeView.SelectedNode.Tag).canDownload)
            {
                Clipboard.SetText(getDownloadURL(plexTreeView.SelectedNode));
                MessageBox.Show("URL Copied to clipboard");
            }
        }

        private string getDownloadURL(TreeNode node)
        {
            return ((Descriptor)node.Tag).getDownloadURL();
        }

        private string[] getDownloadURLs(TreeNode node)
        {
            return getDownloads(node).Select(x => x.getDownloadURL() + "|" + x.downloadFullpath).ToArray();
        }

        private Descriptor[] getDownloads(TreeNode nodes)
        {
            ArrayList strings = new ArrayList();
            if (nodes.Nodes.Count > 0)
            {
                if (nodes.FirstNode.Text == null || nodes.FirstNode.Text == "")
                {
                    nodes.Nodes.Clear();
                    PlexUtils.populateSubNodes(nodes, this);
                }
                foreach (TreeNode node in nodes.Nodes)
                {
                    if (((Descriptor)node.Tag).canDownload)
                    {
                        strings.Add(node.Tag);
                    }
                    else
                    {
                        if (node.Nodes.Count > 0)
                        {
                            if (node.FirstNode.Text == null || node.FirstNode.Text == "")
                            {
                                node.Nodes.Clear();
                                PlexUtils.populateSubNodes(node, this);
                            }
                            strings.AddRange(getDownloads(node));
                        }
                    }
                }
            }
            return (Descriptor[])strings.ToArray(typeof(Descriptor));
        }

        private void plexTreeView_MouseUp(object sender, MouseEventArgs e)
        {
            selected = ((TreeView)sender).GetNodeAt(e.Location);
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                contextMenuStrip1.Show();
        }

        private void RefreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selected.Collapse(false);
            selected.Nodes.Clear();
            selected.Nodes.Add("");
            selected.Expand();
        }

        private void ExpandServers_Click(object sender, EventArgs e)
        {
            foreach (TreeNode n in plexTreeView.Nodes)
            {
                n.Expand();
            }
        }

        private void Search_Click_1(object sender, EventArgs e)
        {
            //MessageBox.Show("Coming Soon");
            Search myNewForm = new Search();
            var dialog = myNewForm.ShowDialog();
            //TODO: Create SearchResults window.
            List<Descriptor> descriptors = new List<Descriptor>();
            foreach (TreeNode t in plexTreeView.Nodes)
            {
                descriptors.Add((Descriptor)t.Tag);
            }
            SearchResults sr = new SearchResults(descriptors, myNewForm.query);
            sr.Show();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            switch (method)
            {
                case RefreshMethod.Login:
                case RefreshMethod.LoginCLI:
                case RefreshMethod.Token:
                    loadServerNodesFromXML(PlexUtils.doTokenLogin(Descriptor.myToken,this));
                    break;
                case RefreshMethod.ServerXmlUrl:
                    loadServerNodesFromXML(PlexUtils.doServerXmlLogin(Descriptor.sourceXmlUrl,this));
                    break;
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (selected != null && selected.Tag != null)
                playInVLCToolStripMenuItem.Visible = ((Descriptor)selected.Tag).canDownload;
            else
                playInVLCToolStripMenuItem.Visible = false;
        }

        private void playInVLCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            RegistryKey key = null;
            try
            {
                using (var lm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
                {
                    key = lm.OpenSubKey(@"SOFTWARE\VideoLAN\VLC\");
                    if (key == null)
                        key = lm.OpenSubKey(@"SOFTWARE\WOW6432Node\VideoLAN\VLC\");
                    if (key == null)
                        MessageBox.Show("Please install VLC to use this feature");
                }
            }
            catch
            {
                MessageBox.Show("Failed to read from registry");
            }
            if (key != null)
            {
                try
                {
                    Process.Start(key.GetValue("").ToString(), getDownloadURL(selected));
                }
                catch
                {
                    MessageBox.Show("Failed to launch VLC.");
                }
            }
        }

        private void downloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DownloadInfo[] di = getDownloads(plexTreeView.SelectedNode).Select(x => new DownloadInfo(x.getDownloadURL(), PlexUtils.MakeValidFileName(x.downloadFilename), PlexUtils.MakeValidFileName(x.subdir))).ToArray();
            if (downloadDialog == null || downloadDialog.IsDisposed)
                downloadDialog = new DownloadDialog(di);
            else
                downloadDialog.enqueue(di);
            downloadDialog.Show();
        }
    }
}
