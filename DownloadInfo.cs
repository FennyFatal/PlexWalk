using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlexWalk
{
    public class DownloadInfo
    {
        public string downloadURL;
        public string downloadFile;
        public string subdir = null;
        public DownloadInfo(string downloadURL, string downloadFile, string downloadFolder)
        {
            this.subdir = downloadFolder;
            this.downloadFile = downloadFile;
            this.downloadURL = downloadURL;
        }
        public override string ToString()
        {
            return (subdir != null ? subdir + "/" : "") + downloadFile;
        }
    }
}
