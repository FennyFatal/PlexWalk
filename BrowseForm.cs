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
    public partial class BrowseForm : Form, PlexWalk.PlexUtils.FormInterface
    {
        private enum RefreshMethod
        {
            ServerXmlUrl,
            Login,
            LoginCLI,
            Token,
        };
        private RefreshMethod method;
        private DownloadDialog downloadDialog = null;
        string ownedPath = Descriptor.libraryBasePath;
        static TreeNode selected = null;
        public BrowseForm(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
                parseArgs(args);
        }
        private Dictionary<string, string> args = new Dictionary<string, string>();

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
                                this.args.Add(
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
                    this.args.Add(last, next);
                    last = null;
                }
            }
            if (last != null)
                throw new ArgumentException(String.Format("Required argument missing for '{0}'", last));
        }
        private TreeNodeCollection parseServers(string servers_xml)
        {
            TreeNode tn = new TreeNode();
            #region ServerData
            using (XmlReader reader = XmlReader.Create(new StringReader(servers_xml)))
            {
                String address;
                String name;
                int port;
                string accessToken = "";
                string scheme;
                string basepath = Descriptor.libraryBasePath;
                while (reader.ReadToFollowing("Server"))
                {
                    if (!reader.MoveToAttribute("name"))
                        continue;
                    name = reader.ReadContentAsString();

                    if (!reader.MoveToAttribute("address"))
                        continue;
                    address = reader.ReadContentAsString();

                    if (!reader.MoveToAttribute("port"))
                        continue;
                    port = reader.ReadContentAsInt();

                    if (!reader.MoveToAttribute("scheme"))
                        continue;
                    scheme = reader.ReadContentAsString();

                    if (reader.MoveToAttribute("accessToken"))
                    {
                        accessToken = reader.ReadContentAsString();
                        accessToken = String.Format("X-Plex-Token={0}", accessToken);
                    }
                    else
                    {
                        accessToken = String.Format("X-Plex-Token={0}", Descriptor.myToken);
                        basepath = ownedPath;
                    }
                    if (reader.MoveToAttribute("owned") && reader.ReadContentAsString() == "1")
                        basepath = ownedPath;

                    TreeNode node = new TreeNode(name);
                    node.Tag = new Descriptor(String.Format("{0}://{1}:{2}", scheme, address, port), accessToken);
                    node.Name = basepath;
                    tn.Nodes.Add(node);
                }
            }
            #endregion
            return tn.Nodes;
        }
        private void BrowseForm_Load(object sender, EventArgs e)
        {

            string parseME = null;
            bool normal_login = true;

            Descriptor.GUID = Guid.NewGuid().ToString();

            if (args.ContainsKey("owned_path"))
                ownedPath = args["owned_path"];

            if (args.ContainsKey("server_xml"))
                parseME = doServerXmlLogin(args["server_xml"].Replace("\"", ""));
            else if (args.ContainsKey("token"))
                parseME = doTokenLogin(args["token"]);

            if (parseME == null)
                parseME = doMetaLogin();

            loadServerNodesFromXML(parseME);
        }

        private void loadServerNodesFromXML(string parseME)
        {
            plexTreeView.Nodes.Clear();
            foreach (TreeNode tn in parseServers(parseME))
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

        private string doServerXmlLogin(string server_xml)
        {
            using (WebClient wc = new System.Net.WebClient())
            {
                string result;
                try
                {
                    result = wc.DownloadString(server_xml);
                    method = RefreshMethod.ServerXmlUrl;
                    Descriptor.sourceXmlUrl = server_xml;
                }
                catch
                {
                    return null;
                }
                return result;
            }
        }

        private string doTokenLogin(string token)
        {
            using (WebClient wc = new System.Net.WebClient())
            {
                string result;
                Descriptor.myToken = token;
                try
                {
                    result = wc.DownloadString("https://plex.tv/pms/servers.xml" + "?X-Plex-Token=" + Descriptor.myToken);
                    method = RefreshMethod.Token;
                }
                catch
                {
                    return null;
                }
                return result;
            }
        }

        private string doMetaLogin()
        {
            using (WebClient wc = new System.Net.WebClient())
            {
                string parseME = null;
                Boolean fail = false;
                do
                {
                    try
                    {
                        if (!fail && args.ContainsKey("username") && args.ContainsKey("password"))
                        {
                            doLoginFromCLI(wc);
                        }
                        else
                        {
                            doLogin(wc);
                        }
                        parseME = wc.DownloadString("https://plex.tv/pms/servers.xml");
                        wc.Headers["X-Plex-Client-Identifier"] = Descriptor.GUID;
                        Descriptor.myToken = parseLogin(wc.UploadString("https://plex.tv/users/sign_in.xml", String.Empty));
                        fail = false;
                    }
                    catch (Exception ex)
                    {
                        fail = true;
                    }
                } while (fail);
                return parseME;
            }
        }

        private void doLoginFromCLI(WebClient wc)
        {
            wc.Credentials = new NetworkCredential(args["username"], args["password"]);
            wc.Headers[HttpRequestHeader.Authorization] = string.Format(
                "Basic {0}",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(args["username"] + ":" + args["password"]))
            );
            method = RefreshMethod.LoginCLI;
        }

        private void doLogin(WebClient wc)
        {
            Login loginform = new Login();
            loginform.ShowDialog();
            if (loginform.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                this.Close();
                return;
            }
            wc.Credentials = loginform.creds;
            wc.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", loginform.headerAuth);
            method = RefreshMethod.Login;
        }

        private string parseLogin(string login_xml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(login_xml)))
            {
                reader.ReadToDescendant("authentication-token");
                reader.Read();
                return reader.ReadContentAsString();
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

        private static string MakeValidFileName(string name)
        {
            if (name == null)
                return name;
            name = name.Substring(name.LastIndexOfAny("\\/".ToCharArray()) + 1);
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "");
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
            MessageBox.Show("Coming Soon");
            //Search myNewForm = new Search();
            //myNewForm.ShowDialog();
            //TODO: Create SearchResults window.
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            switch (method)
            {
                case RefreshMethod.Login:
                case RefreshMethod.LoginCLI:
                case RefreshMethod.Token:
                    loadServerNodesFromXML(doTokenLogin(Descriptor.myToken));
                    break;
                case RefreshMethod.ServerXmlUrl:
                    loadServerNodesFromXML(doServerXmlLogin(Descriptor.sourceXmlUrl));
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
            DownloadInfo[] di = getDownloads(plexTreeView.SelectedNode).Select(x => new DownloadInfo(x.getDownloadURL(), MakeValidFileName(x.downloadFilename), MakeValidFileName(x.subdir))).ToArray();
            if (downloadDialog == null || downloadDialog.IsDisposed)
                downloadDialog = new DownloadDialog(di);
            else
                downloadDialog.enqueue(di);
            downloadDialog.Show();
        }
    }
}
