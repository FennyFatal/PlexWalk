﻿using System;
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
        static TreeNode selected = null;
        public Dictionary<string, string> LaunchArgs = new Dictionary<string, string>();

        private string iniFileName = "PlexWalk.ini";

        public BrowseForm(string[] args)
        {
            InitializeComponent();
            //Create INI file if it does not exist.
            if (!File.Exists(iniFileName))
                File.Create(iniFileName).Close();

            parseINI(File.ReadAllLines(iniFileName));
            
            //Override any values in the ini with values from the command line.
            if (args.Length > 0)
                parseArgs(args);
        }

        private void parseINI(string[] iniLines)
        {
            foreach (string s in iniLines)
            {
                //# as comment character
                if (s.Trim().StartsWith("#"))
                    continue;
                //All args will be split by an =
                string[] chunks = s.Split('=');
                //Allow any kind of quote or spacing around args
                if (chunks.Length == 2)
                    LaunchArgs[chunks[0].Trim("\t \"'".ToCharArray())] = chunks[1].Trim(" \"'".ToCharArray());
            }
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
                Descriptor.libraryOwnedPath = LaunchArgs["owned_path"].TrimEnd('/');

            if (LaunchArgs.ContainsKey("server_xml"))
                parseME = PlexUtils.doServerXmlLogin(LaunchArgs["server_xml"].Replace("\"", ""),this);
            else if (LaunchArgs.ContainsKey("token"))
                parseME = PlexUtils.doTokenLogin(LaunchArgs["token"],this);

            if (parseME == null)
                parseME = PlexUtils.doMetaLogin(this);
            
            //All Attempts at login failed.
            if (parseME == null)
            {
                CloseForm();
                return;
            }

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
            {
                Application.DoEvents();
                src.Nodes.Clear();
            }
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
            {
                Application.DoEvents();
                Parent.Nodes.Add(Child);
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (((Descriptor)plexTreeView.SelectedNode.Tag).canDownload)
            {
                Clipboard.SetText(PlexUtils.getDownloadURL(plexTreeView.SelectedNode));
                MessageBox.Show("URL Copied to clipboard");
            }
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
            new SearchResults(SearchResults.SearchType.Movie, myNewForm.query).Show();
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
            if (((Descriptor)selected.Tag).isSubtitlesNode)
                PlexUtils.PlayInVLC(selected.Parent.PrevNode,selected);
            else
                PlexUtils.PlayInVLC(selected);
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

        private void AnimeButton_Click(object sender, EventArgs e)
        {
            new SearchResults(SearchResults.SearchType.Library, "Anime").Show();
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            doLogout();
        }

        private void doLogout()
        {
            LaunchArgs.Remove("token");
            LaunchArgs.Remove("server_xml");
            doSaveConfig();
            this.Close();
        }

        public void doSaveConfig()
        {
            doSaveConfig(iniFileName);
        }

        private void doSaveConfig(string iniFileName)
        {
            using (FileStream f = File.Open(iniFileName, FileMode.Truncate))
            using (StreamWriter sw = new StreamWriter(f))
            {
                foreach (var arg in LaunchArgs)
                {
                    if (arg.Key.ToLower() == "username" || arg.Key.ToLower() == "password")
                        continue;
                    sw.WriteLine("{0}={1}", arg.Key, arg.Value);
                }
            }
        }

        private void TVSearchButton_Click(object sender, EventArgs e)
        {
            new SearchResults(SearchResults.SearchType.Shows, doSearch().Item2).Show();
        }

        private void searchServersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var values = doSearch();
            new SearchResults(values.Item1,values.Item2).Show();
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SearchResults(SearchResults.SearchType.Library, doSearch().Item2).Show();
        }

        private Tuple<List<Descriptor>,string> doSearch()
        {
            Search myNewForm = new Search();
            var dialog = myNewForm.ShowDialog();
            //TODO: Create SearchResults window.
            List<Descriptor> descriptors = new List<Descriptor>();
            foreach (TreeNode t in plexTreeView.Nodes)
            {
                descriptors.Add((Descriptor)t.Tag);
            }
            return Tuple.Create(descriptors,myNewForm.query);
        }
    }
}
