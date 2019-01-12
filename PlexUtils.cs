using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace PlexWalk
{
    public enum RefreshMethod
    {
        ServerXmlUrl,
        Login,
        LoginCLI,
        Token,
    };
    public interface FormInterface
    {
        void AddNode(TreeNode tn, TreeNode tn2);
    }
    public interface RootFormInterface : FormInterface
    {
        RefreshMethod GetRefreshMethod();
        RefreshMethod SetRefreshMethod(RefreshMethod method);
        Dictionary<string, string> GetLaunchArgs();
        void CloseForm();
    }
    public class Descriptor
    {
        public string userName;
        public string serverName;
        public static string libraryBasePath = "/library/sections";
        public static string libraryOwnedPath = libraryBasePath;
        public static string sourceXmlUrl;
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
    class PlexUtils
    {
        static public string doServerXmlLogin(string server_xml, RootFormInterface rfi)
        {
            using (WebClient wc = new System.Net.WebClient())
            {
                string result;
                try
                {
                    result = wc.DownloadString(server_xml);
                    rfi.SetRefreshMethod(RefreshMethod.ServerXmlUrl);
                    Descriptor.sourceXmlUrl = server_xml;
                }
                catch
                {
                    return null;
                }
                return result;
            }
        }

        static public TreeNodeCollection parseServers(string servers_xml)
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
                        basepath = Descriptor.libraryOwnedPath;
                    }
                    if (reader.MoveToAttribute("owned") && reader.ReadContentAsString() == "1")
                        basepath = Descriptor.libraryOwnedPath;

                    TreeNode node = new TreeNode(name);
                    node.Tag = new Descriptor(String.Format("{0}://{1}:{2}", scheme, address, port), accessToken) 
                    { 
                        serverName = name
                    };
                    node.Name = basepath;
                    tn.Nodes.Add(node);
                }
            }
            #endregion
            return tn.Nodes;
        }

        static public string doTokenLogin(string token, RootFormInterface rfi)
        {
            using (WebClient wc = new System.Net.WebClient())
            {
                string result;
                Descriptor.myToken = token;
                try
                {
                    result = wc.DownloadString("https://plex.tv/pms/servers.xml" + "?X-Plex-Token=" + Descriptor.myToken);
                    rfi.SetRefreshMethod(RefreshMethod.Token);
                }
                catch
                {
                    return null;
                }
                return result;
            }
        }

        public static string doMetaLogin(RootFormInterface rfi)
        {
            var args = rfi.GetLaunchArgs();
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
                            doLoginFromCLI(wc,rfi);
                        }
                        else
                        {
                            doLogin(wc,rfi);
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

        static private void doLoginFromCLI(WebClient wc, RootFormInterface rfi)
        {
            var args = rfi.GetLaunchArgs();
            wc.Credentials = new NetworkCredential(args["username"], args["password"]);
            wc.Headers[HttpRequestHeader.Authorization] = string.Format(
                "Basic {0}",
                Convert.ToBase64String(Encoding.ASCII.GetBytes(args["username"] + ":" + args["password"]))
            );
            rfi.SetRefreshMethod(RefreshMethod.LoginCLI);
        }

        static private void doLogin(WebClient wc, RootFormInterface rfi)
        {
            Login loginform = new Login();
            loginform.ShowDialog();
            if (loginform.DialogResult == System.Windows.Forms.DialogResult.Cancel)
            {
                rfi.CloseForm();
                return;
            }
            wc.Credentials = loginform.creds;
            wc.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", loginform.headerAuth);
            rfi.SetRefreshMethod(RefreshMethod.Login);
        }

        static private string parseLogin(string login_xml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(login_xml)))
            {
                reader.ReadToDescendant("authentication-token");
                reader.Read();
                return reader.ReadContentAsString();
            }
        }

        public static string MakeValidFileName(string name)
        {
            if (name == null)
                return name;
            name = name.Substring(name.LastIndexOfAny("\\/".ToCharArray()) + 1);
            string invalidChars = System.Text.RegularExpressions.Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
            string invalidRegStr = string.Format(@"([{0}]*\.+$)|([{0}]+)", invalidChars);

            return System.Text.RegularExpressions.Regex.Replace(name, invalidRegStr, "");
        }

        public static void populateSubNodes(TreeNode tnode, FormInterface fi)
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
                        query = String.Format("query={0}", Uri.EscapeDataString(dialog.query));
                    }
                }
                string url = ((Descriptor)tnode.Tag).host + tnode.Name + (!tnode.Name.Contains("?") ? '?' : '&') + query + (query.Equals(string.Empty) ? "" : "&") + ((Descriptor)tnode.Tag).token;
                Console.WriteLine(url);
                try
                {
                    xmlString = wc.DownloadString(url);
                }
                catch (Exception ex)
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
                                fi.AddNode(tnode, node);
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
                            if (!reader.MoveToAttribute("key"))
                                continue;
                            key = reader.ReadContentAsString();

                            if (!reader.MoveToAttribute("title"))
                                continue;
                            title = reader.ReadContentAsString();

                            TreeNode node = new TreeNode(title);
                            node.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                            node.Tag = new Descriptor((Descriptor)tnode.Tag);

                            if (!reader.ReadToFollowing("Media"))
                                continue;

                            int width = -1;
                            int height = -1;
                            int size = -1;
                            bool isIndirect = false;

                            if (reader.MoveToAttribute("width"))
                                width = reader.ReadContentAsInt();
                            if (reader.MoveToAttribute("height"))
                                height = reader.ReadContentAsInt();
                            if (reader.MoveToAttribute("size"))
                                height = reader.ReadContentAsInt();
                            if (reader.MoveToAttribute("indirect"))
                                isIndirect = reader.ReadContentAsInt() == 1;

                            if (!reader.ReadToFollowing("Part"))
                                continue;
                            do
                            {
                                if (!reader.MoveToAttribute("key"))
                                    continue;

                                key = reader.ReadContentAsString();
                                string container = String.Empty;

                                if (reader.MoveToAttribute("container"))
                                    container = reader.ReadContentAsString();

                                if (!reader.MoveToAttribute("file"))
                                    continue;

                                Descriptor Tag = new Descriptor((((Descriptor)tnode.Tag).host), ((Descriptor)tnode.Tag).token);
                                Tag.downloadFullpath = reader.ReadContentAsString();
                                Tag.downloadFilename = Tag.downloadFullpath.Substring(Tag.downloadFullpath.LastIndexOf("/") + 1);
                                if (Tag.downloadFilename == String.Empty)
                                    Tag.downloadFilename = String.Format("{0}.{1}", title, container);
                                title = String.Format("Download {0} ({1}x{2}) {3}", Tag.downloadFilename, width, height, size);
                                TreeNode subnode = new TreeNode(title);
                                Tag.downloadUrl = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                Tag.canDownload = true;
                                subnode.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                subnode.Tag = Tag;
                                node.Nodes.Add(subnode);

                            }
                            while (reader.ReadToNextSibling("Part"));
                            fi.AddNode(tnode, node);
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
                                    //TODO: are there other types we need here?
                                    case "episode":
                                        if (reader.MoveToAttribute("index"))
                                            episodeNumber = reader.ReadContentAsString();
                                        break;
                                }
                            }
                            if (!reader.MoveToAttribute("key"))
                                continue;
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
                            if (!reader.ReadToFollowing("Media"))
                                continue;

                            int width = -1;
                            int height = -1;
                            int size = -1;
                            bool isIndirect = false;
                            if (reader.MoveToAttribute("width"))
                                width = reader.ReadContentAsInt();
                            if (reader.MoveToAttribute("height"))
                                height = reader.ReadContentAsInt();
                            if (reader.MoveToAttribute("size"))
                                height = reader.ReadContentAsInt();
                            if (reader.MoveToAttribute("indirect"))
                                isIndirect = reader.ReadContentAsInt() == 1;

                            if (!reader.ReadToFollowing("Part"))
                                continue;

                            do
                            {
                                if (!reader.MoveToAttribute("key"))
                                    continue;
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
                                    title = String.Format("Download {0} ({1}x{2})", Tag.downloadFilename, width, height, size);
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
                            while (reader.ReadToNextSibling("Part"));
                            fi.AddNode(tnode, node);
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
                            if (!reader.MoveToAttribute("key"))
                                continue;

                            key = reader.ReadContentAsString();

                            if (!reader.MoveToAttribute("title"))
                                continue;

                            title = reader.ReadContentAsString();

                            if (reader.MoveToAttribute("grandparentTitle"))
                            {
                                artist = reader.ReadContentAsString();
                                title = string.Format("{0} - {1}", artist, title);
                            }
                            if (reader.MoveToAttribute("parentTitle"))
                                album = reader.ReadContentAsString();

                            TreeNode node = new TreeNode(title);
                            node.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                            node.Tag = tnode.Tag;

                            if (!reader.ReadToFollowing("Media"))
                                continue;
                            int duration = -1;

                            if (reader.MoveToAttribute("duration"))
                                duration = reader.ReadContentAsInt();

                            if (!reader.ReadToFollowing("Part"))
                                continue;

                            do
                            {
                                if (!reader.MoveToAttribute("key"))
                                    continue;
                                key = reader.ReadContentAsString();
                                string container = String.Empty;
                                if (reader.MoveToAttribute("container"))
                                    container = reader.ReadContentAsString();
                                if (!reader.MoveToAttribute("file"))
                                    continue;

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
                                    if (artist != null)
                                        Tag.subdir = String.Format("{0} - {1}", artist, album);
                                    else
                                        Tag.subdir = album;

                                TreeNode subnode = new TreeNode(title);
                                Tag.downloadUrl = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                Tag.canDownload = true;
                                subnode.Name = key.StartsWith("/") ? key : (string)tnode.Name + '/' + key;
                                subnode.Tag = Tag;
                                node.Nodes.Add(subnode);
                            }
                            while (reader.ReadToNextSibling("Part"));
                            fi.AddNode(tnode, node);
                        }
                    }
                }
                #endregion
            }
            if (tnode.Name == Descriptor.libraryBasePath)
            {
                var node = new TreeNode("Search") { Tag = tnode.Tag, Name = "/search" };
                ((Descriptor)node.Tag).isSearchNode = true;
                node.Nodes.Add(new TreeNode());
                fi.AddNode(tnode, node);
            }
        }
    }
}
