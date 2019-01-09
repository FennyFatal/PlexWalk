﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace PlexWalk
{
    public partial class DownloadDialog : Form
    {
        public bool Autostart = false;
        private bool isDownloading = false;
        private string path;
        private ObservableQueue<DownloadInfo> myDownloadInfo = new ObservableQueue<DownloadInfo>();
        Regex DurationRegex = new Regex(@"Duration: (\d{2,4}):(\d{2}):(\d{2})[.](\d{2})");
        Regex TimeRegex = new Regex(@"time=(\d{2,4}):(\d{2}):(\d{2})[.](\d{2})");
        public DownloadDialog(DownloadInfo[] downloads)
        {
            foreach (DownloadInfo di in downloads)
            {
                myDownloadInfo.Enqueue(di);
            }
            InitializeComponent();
            path = null;
            using (FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog())
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    path = folderBrowserDialog1.SelectedPath;
                }
            this.DownloadList.DataSource = myDownloadInfo;
        }

        private void DownloadDialog_Load(object senderd, EventArgs e1)
        {
            if (Autostart)
                StartDownloads();
        }

        private void StartDownloads()
        {
            if (path == null)
            {
                Close();
                return;
            }
            this.progressBar2.Maximum = myDownloadInfo.Count() + 1;
            using (WebClient Client = new WebClient())
            {
                Client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    SetProgressCircle(this.progressBar3, e.ProgressPercentage);
                    SetProgressCircle(this.progressBar4, e.ProgressPercentage);
                });

                Client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler
                (delegate (object sender, System.ComponentModel.AsyncCompletedEventArgs e)
                {
                    isDownloading = false;
                });
                (new Thread(() =>
                {
                    doDownload(Client);
                })).Start();
            }
        }
        delegate void SetTextDelegate(Control c, string text);
        delegate void CloseFormDelegate();

        private void CloseForm()
        {
            if (this.InvokeRequired)
            {
                var d = new CloseFormDelegate(CloseForm);
                this.Invoke(d);
            }
            else
            {
                this.Close();
            }
        }

        private void SetText(Control c, string text)
        {
            if (c.InvokeRequired)
            {
                var d = new SetTextDelegate(SetText);
                this.Invoke(d, new object[] { c, text });
            }
            else
            {
                c.Text = text;
            }
        }

        delegate void SetProgressDelegate(ProgressBar p, int value);

        private void SetMax(ProgressBar p, int value)
        {
            if (p.InvokeRequired)
            {
                var d = new SetProgressDelegate(SetMax);
                this.Invoke(d, new object[] { p, value });
            }
            else
            {
                p.Maximum = value;
            }
        }
        private void SetProgress(ProgressBar p, int value)
        {
            if (p.InvokeRequired)
            {
                var d = new SetProgressDelegate(SetProgress);
                this.Invoke(d, new object[] { p, value });
            }
            else
            {
                try { p.Value = value; }
                catch { }
            }
        }
        delegate void SetProgressCircleDelegate(iTalk.iTalk_ProgressBar p, int value);
        private void SetProgressCircle(iTalk.iTalk_ProgressBar p, int value)
        {
            if (p.InvokeRequired)
            {
                var d = new SetProgressCircleDelegate(SetProgressCircle);
                this.Invoke(d, new object[] { p, value });
            }
            else
            {
                try { p.Value = value; }
                catch { }
            }
        }
        private void downloadM3u8(WebClient Client, DownloadInfo di)
        {
            using (Process FFMPEGEncode = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "ffmpeg.exe",
                    Arguments = String.Format(@"-y -i ""{0}"" -c copy -bsf:a aac_adtstoasc ""{1}""", di.downloadURL, path + (di.subdir == null ? "\\" : "\\" + di.subdir + "\\") + di.downloadFile.Replace("mpegts", "mp4").Replace("m3u8", "mp4")),
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            })
            {
                FFMPEGEncode.ErrorDataReceived += new DataReceivedEventHandler
                    (delegate (object sender, DataReceivedEventArgs e)
                    {
                        if (e.Data == null)
                            ;
                        else
                        {
                            if (DurationRegex.IsMatch(e.Data))
                            {
                                var match = DurationRegex.Match(e.Data);
                                Console.WriteLine("Matched: " + match.Groups[0]);
                                SetProgressCircle(progressBar3, 0);
                                SetProgressCircle(progressBar4, 0);
                            }

                        }
                    }
                    );
                FFMPEGEncode.Start();
                FFMPEGEncode.BeginErrorReadLine();
                while (!FFMPEGEncode.WaitForExit(2))
                {
                    Application.DoEvents();
                }
            }
            /*
            using (WebClient TSClient = new WebClient())
            {
                TSClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler
                (delegate(object sender, DownloadStringCompletedEventArgs e)
                {
                    downloadM3u8(e.Result, (DownloadInfo)e.UserState);
                });
                TSClient.DownloadStringAsync(new Uri(di.downloadURL), di);
            }
                */
        }

        delegate DownloadInfo deQueueDelegate();

        DownloadInfo deQueue()
        {
            if (this.InvokeRequired)
            {
                var d = new deQueueDelegate(deQueue);
                return (DownloadInfo)this.Invoke(d);
            }
            else
            {
                return myDownloadInfo.Dequeue();
            }
        }

        private void doDownload(WebClient Client)
        {
            while (myDownloadInfo.Any())
            {
                SetProgress(progressBar2, progressBar2.Maximum - myDownloadInfo.Count);
                DownloadInfo di = deQueue();
                SetText(label1, di.downloadFile);
                makeSureExists(path + (di.subdir == null ? "\\" : "\\" + di.subdir + "\\"));
                if (di.downloadURL.Contains(".m3u8"))
                {
                    downloadM3u8(Client, di);
                }
                else
                {
                    isDownloading = true;
                    Client.DownloadFileAsync(new Uri(di.downloadURL), path + (di.subdir == null ? "\\" : "\\" + di.subdir + "\\") + di.downloadFile);
                    while (isDownloading)
                    {
                        Thread.Sleep(2);
                        Application.DoEvents();
                    }
                }
            }
            this.CloseForm();
            return;
        }
        /*
        BinaryWriter output;

        private void downloadM3u8(string p, DownloadInfo downloadInfo)
        {
            using (WebClient M3u8Client = new WebClient())
            {
                output = new System.IO.BinaryWriter(File.OpenWrite(path + (downloadInfo.subdir == null ? "\\" : "\\" + downloadInfo.subdir + "\\") + downloadInfo.downloadFile));
                {
                    var downloadinfo = parseM3u8(p);
                    this.progressBar1.Maximum = downloadinfo.Count;
                    M3u8Client.DownloadDataCompleted += new DownloadDataCompletedEventHandler
                    (delegate (object source, DownloadDataCompletedEventArgs e)
                    {
                        output.Write(e.Result);
                        output.Flush();
                        var remaining = (List<DownloadInfo>)e.UserState;
                        if (remaining.Count > 0)
                        {
                            downloadNextM3u8ItemFromList(M3u8Client, downloadinfo);
                        }
                        else
                        {
                            output.Close();
                            output.Dispose();
                        }
                    }
                    );
                    downloadNextM3u8ItemFromList(M3u8Client, downloadinfo);
                }
            }
        }
        private void downloadNextM3u8ItemFromList(WebClient M3u8Client, List<DownloadInfo> remaining)
        {
            this.progressBar1.Value = this.progressBar1.Maximum - remaining.Count;
            var di = remaining.First();
            remaining.Remove(di);
            M3u8Client.DownloadDataAsync(new Uri(di.downloadURL), remaining);
        }
        private List<DownloadInfo> parseM3u8(string p)
        {
            ArrayList parts = new ArrayList();
            foreach (string s in p.Split("\r\n".ToCharArray()))
            {
                Console.WriteLine(s);
                if (s.StartsWith("http://") || s.StartsWith("https://"))
                    parts.Add(new DownloadInfo(s,null,null));
            }
            return parts.Cast<DownloadInfo>().ToList();
        }
         */
        private void makeSureExists(string p)
        {
            try
            {
                System.IO.Directory.CreateDirectory(p);
            }
            catch { }
        }

        private void DownloadList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void DownloadList_MouseUp(object sender, MouseEventArgs e)
        {
            ((ListBox)sender).SelectedIndex = ((ListBox)sender).IndexFromPoint(e.Location);
        }

        private void Start_Click(object sender, EventArgs e)
        {
            StartDownloads();
            ((Button)sender).Enabled = false;
        }

        internal void enqueue(DownloadInfo[] di)
        {
            foreach (DownloadInfo d in di)
            {
                myDownloadInfo.Enqueue(d);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            myDownloadInfo.Remove((DownloadInfo)DownloadList.SelectedItem);
        }
        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
