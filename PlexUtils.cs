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
    public class Descriptor
    {
        public static string libraryBasePath = "/library/sections";
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
        public interface FormInterface
        {
            void AddNode(TreeNode tn, TreeNode tn2);
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
