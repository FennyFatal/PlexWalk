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

namespace PlexWalk
{
    public partial class BrowseForm : Form
    {
        private DownloadDialog downloadDialog = null;
        const string libraryBasePath = "/library/sections";
        static TreeNode selected = null;
        public class Descriptor
        {
            public bool isSearchNode;
            public static string GUID;
            public static string myToken;
            public string host;
            public string token;
            public Boolean canDownload = false;
            public bool isIndirect = false;
            public string downloadUrl;
            public string downloadFilename;
            public string downloadFullpath;
            public string ShowTitle = null;
            public string Title;
            public string subdir = null;
            public string seasonNumber = null;
            public string episodeNumber = null;
            public override string ToString()
            {
                return Title;
            }
            public Descriptor(string host, string token)
            {
                this.host = host;
                this.token = token;
            }
            public Descriptor(Descriptor d)
            {
                this.host = d.host;
                this.token = d.token;
                this.isSearchNode = d.isSearchNode;
                this.canDownload = d.canDownload;
                this.downloadUrl = d.downloadUrl;
                this.downloadFilename = d.downloadFilename;
                this.Title = d.Title;
                this.seasonNumber = d.seasonNumber; // Inherit season information from parent nodes.
                this.episodeNumber = d.episodeNumber;
                this.ShowTitle = d.ShowTitle;
            }
            public string getDownloadURL()
            {
                if (!downloadUrl.StartsWith("/"))
                    return downloadUrl;
                return host + downloadUrl + (token != "" ? (!downloadUrl.Contains("?") ? '?' : '&') + token : "");
            }
        }
        public string parseME;
        public string authME;
        public BrowseForm(string[] args)
        {
            InitializeComponent();
            if (args.Length > 0)
                parseArgs(args);
        }
        private Dictionary<string,string> args = new Dictionary<string,string>();

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
                string basepath;
                while (reader.ReadToFollowing("Server"))
                {
                    if (reader.MoveToAttribute("name"))
                    {
                        name = reader.ReadContentAsString();
                        if (reader.MoveToAttribute("address"))
                        {
                            address = reader.ReadContentAsString();
                            {
                                if (reader.MoveToAttribute("port"))
                                {
                                    port = reader.ReadContentAsInt();
                                    if (reader.MoveToAttribute("scheme"))
                                    {
                                        scheme = reader.ReadContentAsString();
                                        if (reader.MoveToAttribute("accessToken"))
                                        {
                                            accessToken = reader.ReadContentAsString();
                                            accessToken = String.Format("X-Plex-Token={0}", accessToken);
                                            basepath = libraryBasePath;
                                        }
                                        else
                                        {
                                            accessToken = String.Format("X-Plex-Token={0}", Descriptor.myToken);
                                            basepath = "";
                                        }
                                        TreeNode node = new TreeNode(name);
                                        node.Tag = new Descriptor(String.Format("{0}://{1}:{2}", scheme, address, port), accessToken);
                                        node.Name = basepath;
                                        tn.Nodes.Add(node);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion
            return tn.Nodes;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
        Descriptor.GUID = Guid.NewGuid().ToString();
            using (WebClient wc = new System.Net.WebClient())
            {
                bool noToken = true;
                if (args.ContainsKey("server_xml"))
                {
                    noToken = false;
                    parseME = wc.DownloadString(args["server_xml"].Replace("\"",""));
                }
                else if (args.ContainsKey("token"))
                {
                    noToken = false;
                    Descriptor.myToken = args["token"];
                    try
                    {
                        parseME = wc.DownloadString("https://plex.tv/pms/servers.xml" + "?X-Plex-Token=" + args["token"]);
                    }
                    catch
                    {
                        noToken = true;
                    }
                }
                if (noToken)
                {
                    Boolean fail = false;
                    do
                    {
                        {
                            try
                            {
                                if (!fail && args.ContainsKey("username") && args.ContainsKey("password"))
                                {
                                    wc.Credentials = new NetworkCredential(args["username"], args["password"]);
                                    wc.Headers[HttpRequestHeader.Authorization] = string.Format(
                                        "Basic {0}",
                                        Convert.ToBase64String(Encoding.ASCII.GetBytes(args["username"] + ":" + args["password"]))
                                    );

                                }
                                else
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
                                }
                                parseME = wc.DownloadString("https://plex.tv/pms/servers.xml");
                                wc.Headers["X-Plex-Client-Identifier"] = Descriptor.GUID;
                                authME = wc.UploadString("https://plex.tv/users/sign_in.xml", String.Empty);
                                fail = false;
                            }
                            catch (Exception ex)
                            {
                                fail = true;
                            }
                        }
                    } while (fail);
                    Descriptor.myToken = parseLogin(authME);
                }
            }
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
            TreeNode src = e.Node;
            if (src.FirstNode.Text == null || src.FirstNode.Text == "")
            {
                src.Nodes.Clear();
                try
                {
                    populateSubNodes(src);
                }
                catch
                {
                }
            }
        }
        private void populateSubNodes(TreeNode tnode)
        {
            using (WebClient wc = new System.Net.WebClient())
            {
                String xmlString = null;
                string query = String.Empty;
                if (((Descriptor)tnode.Tag).isSearchNode)
                {
                    Search dialog = new Search();
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        query = String.Format("query={0}",Uri.EscapeDataString(dialog.query));
                    }
                }
                string url = ((Descriptor)tnode.Tag).host + tnode.Name + (!tnode.Name.Contains("?") ? '?' : '&') + query + (query.Equals(string.Empty)? "" : "&") + ((Descriptor)tnode.Tag).token;
                Console.WriteLine(url);
                try
                {
                    xmlString = wc.DownloadString(url);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return;
                }
                #region MediaContainer
                string title2 = null;
                int count = 0;
                if (xmlString.Contains("<MediaContainer "))
                {
                    using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                    {
                        while (reader.ReadToFollowing("MediaContainer"))
                        {
                            if (reader.MoveToAttribute("title2"))
                            {
                                title2 = reader.ReadContentAsString();
                                count++;
                            }
                        }
                    }
                }
                #endregion
                #region FolderData
                using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                {
                    String key;
                    String title;
                    while (reader.ReadToFollowing("Directory"))
                    {
                        string seasonNumber = null;
                        string ShowName = null;
                        if (reader.MoveToAttribute("type"))
                        {
                            switch (reader.ReadContentAsString())
                            {
                                case "season":
                                    if (reader.MoveToAttribute("index"))
                                    {
                                        seasonNumber = reader.ReadContentAsString();
                                    }
                                    break;
                                case "show":
                                    if (reader.MoveToAttribute("title"))
                                    {
                                        ShowName = reader.ReadContentAsString();
                                    }
                                    break;
                            }
                        }
                        if (reader.MoveToAttribute("key"))
                        {
                            key = reader.ReadContentAsString();
                            if (reader.MoveToAttribute("title") || reader.MoveToAttribute("name"))
                            {
                                title = reader.ReadContentAsString();
                                {
                                    TreeNode node = new TreeNode(title);
                                    node.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                    if (title.Equals("search") || (reader.MoveToAttribute("search") && reader.ReadContentAsInt() == 1))
                                    {
                                        node.Tag = new Descriptor((Descriptor)tnode.Tag);
                                        ((Descriptor)node.Tag).isSearchNode = true;
                                    }
                                    else
                                    {
                                        node.Tag = new Descriptor((Descriptor)tnode.Tag);
                                        ((Descriptor)node.Tag).isSearchNode = false;
                                    }
                                    if (ShowName != null)
                                        ((Descriptor)node.Tag).ShowTitle = ShowName;
                                    if (seasonNumber != null)
                                        ((Descriptor)node.Tag).seasonNumber = seasonNumber;
                                    node.Nodes.Add(new TreeNode());
                                    tnode.Nodes.Add(node);
                                }
                            }
                        }
                    }
                }
                #endregion
                #region PhotoData
                if (xmlString.Contains("<Photo "))
                {
                    using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                    {
                        String key;
                        String title;
                        while (reader.ReadToFollowing("Photo"))
                        {
                            if (reader.MoveToAttribute("key"))
                            {
                                key = reader.ReadContentAsString();
                                if (reader.MoveToAttribute("title"))
                                {
                                    title = reader.ReadContentAsString();
                                    TreeNode node = new TreeNode(title);
                                    node.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                    node.Tag = new Descriptor((Descriptor)tnode.Tag);
                                    if (reader.ReadToFollowing("Media"))
                                    {
                                        int width = -1;
                                        int height = -1;
                                        bool isIndirect = false;
                                        if (reader.MoveToAttribute("width"))
                                        {
                                            width = reader.ReadContentAsInt();
                                        }
                                        if (reader.MoveToAttribute("height"))
                                        {
                                            height = reader.ReadContentAsInt();
                                        }
                                        if (reader.MoveToAttribute("indirect"))
                                        {
                                            isIndirect = reader.ReadContentAsInt() == 1;
                                        }
                                        if (reader.ReadToFollowing("Part"))
                                        {
                                            do 
                                            {
                                                if (reader.MoveToAttribute("key"))
                                                {
                                                    key = reader.ReadContentAsString();
                                                    string container = String.Empty;
                                                    if (reader.MoveToAttribute("container"))
                                                        container = reader.ReadContentAsString();
                                                    if (reader.MoveToAttribute("file"))
                                                    {
                                                        Descriptor Tag = new Descriptor((((Descriptor)tnode.Tag).host), ((Descriptor)tnode.Tag).token);
                                                        Tag.downloadFullpath = reader.ReadContentAsString();
                                                        Tag.downloadFilename = Tag.downloadFullpath.Substring(Tag.downloadFullpath.LastIndexOf("/") + 1);
                                                        if (Tag.downloadFilename == String.Empty)
                                                            Tag.downloadFilename = String.Format("{0}.{1}", title, container);
                                                        title = String.Format("Download {0} ({1}x{2})", Tag.downloadFilename, width, height);
                                                        TreeNode subnode = new TreeNode(title);
                                                        Tag.downloadUrl = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                                        Tag.canDownload = true;
                                                        subnode.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                                        subnode.Tag = Tag;
                                                        node.Nodes.Add(subnode);
                                                    }
                                                }
                                            }
                                            while (reader.ReadToNextSibling("Part"));
                                        }
                                    }
                                    tnode.Nodes.Add(node);
                                }
                            }
                        }
                    }
                }
                #endregion
                #region VideoData
                if (xmlString.Contains("<Video "))
                {
                    using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                    {
                        String key;
                        String title;
                        while (reader.ReadToFollowing("Video"))
                        {
                            String episodeNumber = null;
                            if (reader.MoveToAttribute("type"))
                            {
                                switch (reader.ReadContentAsString())
                                {
                                    case "episode":
                                        if (reader.MoveToAttribute("index"))
                                        {
                                            episodeNumber = reader.ReadContentAsString();
                                        }
                                        break;
                                }
                            }
                            if (reader.MoveToAttribute("key"))
                            {
                                key = reader.ReadContentAsString();
                                if (reader.MoveToAttribute("title"))
                                {
                                    title = reader.ReadContentAsString();
                                }
                                else if (reader.MoveToAttribute("type") && reader.ReadContentAsString().Equals("clip"))
                                {
                                    title = tnode.Text;
                                }
                                else
                                {
                                    title = "Show Video";
                                }
                                TreeNode node = new TreeNode(title);
                                node.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                node.Tag = new Descriptor((Descriptor)tnode.Tag);
                                ((Descriptor)node.Tag).episodeNumber = episodeNumber;
                                if (reader.ReadToFollowing("Media"))
                                {
                                    int width = -1;
                                    int height = -1;
                                    bool isIndirect = false;
                                    if (reader.MoveToAttribute("width"))
                                    {
                                        width = reader.ReadContentAsInt();
                                    }
                                    if (reader.MoveToAttribute("height"))
                                    {
                                        height = reader.ReadContentAsInt();
                                    }
                                    if (reader.MoveToAttribute("indirect"))
                                        isIndirect = reader.ReadContentAsInt() == 1;
                                    if (reader.ReadToFollowing("Part"))
                                    {
                                        do
                                        {
                                            if (reader.MoveToAttribute("key"))
                                            {
                                                key = reader.ReadContentAsString();
                                                string container = null;
                                                if (reader.MoveToAttribute("container"))
                                                {
                                                    container = reader.ReadContentAsString();
                                                }
                                                string filename = key;
                                                if (isIndirect)
                                                {
                                                    ((Descriptor)node.Tag).isIndirect = true;
                                                    node.Nodes.Add("");
                                                    node.Name = key;
                                                }
                                                else if (key.StartsWith("http") || (reader.MoveToAttribute("file") && !(filename = reader.ReadContentAsString()).Equals(String.Empty)))
                                                {
                                                    Descriptor Tag = new Descriptor((((Descriptor)tnode.Tag).host), ((Descriptor)tnode.Tag).token);
                                                    if (((Descriptor)tnode.Tag).seasonNumber != null)
                                                    {
                                                        Tag.seasonNumber = ((Descriptor)tnode.Tag).seasonNumber;
                                                    }
                                                    if (((Descriptor)tnode.Tag).episodeNumber != null)
                                                    {
                                                        Tag.episodeNumber = ((Descriptor)tnode.Tag).episodeNumber;
                                                    }
                                                    if (((Descriptor)tnode.Tag).ShowTitle != null)
                                                    {
                                                        Tag.ShowTitle = ((Descriptor)tnode.Tag).ShowTitle;
                                                    }
                                                    Tag.downloadFullpath = filename;
                                                    Tag.downloadFilename = Tag.downloadFullpath.Substring(Tag.downloadFullpath.LastIndexOf("/") + 1);
                                                    if (Tag.downloadFilename.Contains('?') && container != null)
                                                    {
                                                        if (title2 != null)
                                                            Tag.downloadFilename = String.Format("{0}.{1}", title2, container);
                                                        else
                                                            Tag.downloadFilename = String.Format("{0}.{1}", tnode.Text, container);
                                                    }
                                                    title = String.Format("Download {0} ({1}x{2})", Tag.downloadFilename, width, height);
                                                    if (Tag.ShowTitle != null && Tag.episodeNumber != null)
                                                    {
                                                        if (Tag.seasonNumber != null)
                                                            Tag.subdir = String.Format("{0}.S{1:00}E{2:00}", Tag.ShowTitle, Int32.Parse(Tag.seasonNumber), Int32.Parse(Tag.episodeNumber));
                                                        else
                                                            Tag.subdir = String.Format("{0}.S01E{2:00}", Tag.ShowTitle, Tag.seasonNumber, Int32.Parse(Tag.episodeNumber));
                                                    }
                                                    TreeNode subnode = new TreeNode(title);
                                                    if (key.StartsWith("http"))
                                                        Tag.downloadUrl = key;
                                                    else
                                                        Tag.downloadUrl = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                                    Tag.canDownload = true;
                                                    Tag.isIndirect = isIndirect;
                                                    subnode.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                                    subnode.Tag = Tag;
                                                    if (isIndirect)
                                                        subnode.Nodes.Add("");
                                                    node.Nodes.Add(subnode);
                                                }
                                            }
                                        }
                                        while (reader.ReadToNextSibling("Part"));
                                    }
                                }
                                tnode.Nodes.Add(node);
                            }
                        }
                    }
                }
                #endregion
                #region TrackData
                if (xmlString.Contains("<Track "))
                {
                    using (XmlReader reader = XmlReader.Create(new StringReader(xmlString)))
                    {
                        String key;
                        String title;
                        String album = null;
                        String artist = null;
                        while (reader.ReadToFollowing("Track"))
                        {
                            if (reader.MoveToAttribute("key"))
                            {
                                key = reader.ReadContentAsString();
                                if (reader.MoveToAttribute("title"))
                                {
                                    title = reader.ReadContentAsString();
                                    if (reader.MoveToAttribute("grandparentTitle"))
                                    {
                                        artist = reader.ReadContentAsString();
                                        title = string.Format("{0} - {1}", artist, title);
                                    }
                                    if (reader.MoveToAttribute("parentTitle"))
                                    {
                                        album = reader.ReadContentAsString();
                                    }
                                    TreeNode node = new TreeNode(title);
                                    node.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                    node.Tag = tnode.Tag;
                                    if (reader.ReadToFollowing("Media"))
                                    {
                                        int duration = -1;
                                        if (reader.MoveToAttribute("duration"))
                                        {
                                            duration = reader.ReadContentAsInt();
                                        }
                                        if (reader.ReadToFollowing("Part"))
                                        {
                                            do
                                            {
                                                if (reader.MoveToAttribute("key"))
                                                {
                                                    key = reader.ReadContentAsString();
                                                    string container = String.Empty;
                                                    if (reader.MoveToAttribute("container"))
                                                        container = reader.ReadContentAsString();
                                                    if (reader.MoveToAttribute("file"))
                                                    {
                                                        Descriptor Tag = new Descriptor((((Descriptor)tnode.Tag).host), ((Descriptor)tnode.Tag).token);
                                                        Tag.downloadFullpath = reader.ReadContentAsString();
                                                        Tag.downloadFilename = Tag.downloadFullpath.Substring(Tag.downloadFullpath.LastIndexOf("/") + 1);
                                                        if (Tag.downloadFilename == String.Empty)
                                                            Tag.downloadFilename = String.Format("{0}.{1}", title, container);
                                                        if (duration > 0)
                                                            title = String.Format("Download {0} ({1})", Tag.downloadFilename, duration);
                                                        else
                                                            title = String.Format("Download {0}", Tag.downloadFilename);
                                                        if (album != null)
                                                        {
                                                            if (artist != null)
                                                                Tag.subdir = String.Format("{0} - {1}", artist, album);
                                                            else
                                                                Tag.subdir = album;
                                                        }
                                                        TreeNode subnode = new TreeNode(title);
                                                        Tag.downloadUrl = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                                        Tag.canDownload = true;
                                                        subnode.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                                        subnode.Tag = Tag;
                                                        node.Nodes.Add(subnode);
                                                    }
                                                }
                                            }
                                            while (reader.ReadToNextSibling("Part"));
                                        }
                                    }
                                    tnode.Nodes.Add(node);
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }
        private static string MakeValidFileName(string name)
        {
            if (name == null)
                return name;
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
            else
            {
                DownloadInfo[] di = getDownloads(plexTreeView.SelectedNode).Select(x => new DownloadInfo(x.getDownloadURL(), MakeValidFileName(x.downloadFilename), MakeValidFileName(x.subdir))).ToArray();
                if (downloadDialog == null || downloadDialog.IsDisposed)
                    downloadDialog = new DownloadDialog(di);
                else
                    downloadDialog.enqueue(di);
                downloadDialog.Show();
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
                    populateSubNodes(nodes);
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
                                populateSubNodes(node);
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

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            selected.Collapse(false);
            selected.Nodes.Clear();
            selected.Nodes.Add("");
            selected.Expand();
        }
    }
}
