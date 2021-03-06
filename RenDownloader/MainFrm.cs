﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace RenDownloader
{
    public partial class MainFrm : Form
    {
        private Thread downloadThread = null;

        public static CookieContainer cookieContainer = new CookieContainer();

        private String ffmpegPath = "ffmpeg.exe";

        public MainFrm()
        {
            InitializeComponent();
        }

      
        private void btnSelectSaveDir_Click(object sender, EventArgs e)
        {
            txtSavePath.Text = String.Empty;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                txtSavePath.Text = dialog.SelectedPath;
            }
        }

        private void txtStartDownload_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtEpisodeUrls.Text) || String.IsNullOrEmpty(txtSavePath.Text)) return;

            if (downloadThread != null)
            {
                downloadThread.Abort();
                downloadThread = null;
                UpdateBtn("Download");
                Application.DoEvents();
                return;
            }

            progress.Value = 0;
            UpdateBtn("Stop");
            Application.DoEvents();

            if (cbWebSite.Text == "ren.tv")
            {
                List<List<String>> chunks = new List<List<string>>();
                downloadThread = new Thread(() =>
                {
                    chunks = downloadRenTvFiles();
                    encodeFiles(chunks);
                });
                downloadThread.Start();
            }
            else if (cbWebSite.Text == "kadu.ru")
            {
                List<List<String>> series = new List<List<String>>();
                downloadThread = new Thread(() =>
                {
                    series = downloadKaduRuFiles();
                    //encodeFiles(chunks);
                });
                downloadThread.SetApartmentState(ApartmentState.STA);
                downloadThread.Start();
            }
        }

        private List<List<String>> downloadRenTvFiles()
        {
            String saveTo = txtSavePath.Text;
            var chunks = new List<List<String>>(); 
            List<String> chunkUrls = prepareRenTvEpisodesChunkUrls(); 
            if (chunkUrls.Count == 0) return chunks;
            int incPerChunk = 100 / chunkUrls.Count;
            float progr = 0;
            if (!Directory.Exists(saveTo))
            {
                Directory.CreateDirectory(saveTo);
            }
            int startSeed = 1;
            Int32.TryParse(txtNumberSeed.Text, out startSeed);
            for (int i = 0; i < chunkUrls.Count; i++)
            {
                String epDir = saveTo + "\\" + "episode" + (i + startSeed);
                if (!Directory.Exists(epDir))
                {
                    Directory.CreateDirectory(epDir);
                }
                List<String> fileNames = downloadAndProcessChunklist(chunkUrls[i]);

                float incPerFile = incPerChunk * 1.0f / fileNames.Count;
                List<String> localChunks = new List<string>();
                foreach (var fileName in fileNames)
                {
                    String file = downloadFileToFolder(chunkUrls[i].Replace("chunklist.m3u8", fileName), epDir);
                    localChunks.Add(file);
                    progr += incPerFile;
                    UpdateProgress((int)Math.Round(progr));
                }
                chunks.Add(localChunks);
            }
            UpdateProgress(100);
            UpdateBtn("Encoding");
            Application.DoEvents();
            return chunks;
        }

        private List<List<String>> downloadKaduRuFiles()
        {
            String saveTo = txtSavePath.Text;
            var chunks = new List<List<String>>();
            List<String> episodesUrls = prepareKaduRuSeriesUrls();
            if (episodesUrls.Count == 0) return chunks;
            int incPerChunk = 100 / episodesUrls.Count;
            float progr = 0;
            if (!Directory.Exists(saveTo))
            {
                Directory.CreateDirectory(saveTo);
            }
            int startSeed = 1;
            Int32.TryParse(txtNumberSeed.Text, out startSeed);
            for (int i = 0; i < episodesUrls.Count; i++)
            {
                String epDir = saveTo + "\\" + "episode" + (i + startSeed);
                if (!Directory.Exists(epDir))
                {
                    Directory.CreateDirectory(epDir);
                }
                List<String> fileNames = downloadAndProcessKaduEpisodes(episodesUrls[i]);

                float incPerFile = incPerChunk * 1.0f / fileNames.Count;
                List<String> localChunks = new List<string>();
                foreach (var fileName in fileNames)
                {
                    String file = downloadKaduVideoToFolder(fileName, epDir);
                    localChunks.Add(file);
                    progr += incPerFile;
                    UpdateProgress((int)Math.Round(progr));
                }
                chunks.Add(localChunks);
            }
            UpdateProgress(100);
            UpdateBtn("Encoding");
            Application.DoEvents();
            return chunks;
        }

        private void encodeFiles(List<List<String>> fileNames)
        {
            String saveTo = txtSavePath.Text;
            int incPerEpisode = 100 / fileNames.Count;
            float progr = 0;
            int startSeed = 1;
            Int32.TryParse(txtNumberSeed.Text, out startSeed);
            UpdateProgress(0);
            for (int i = 0; i < fileNames.Count; i++)
            {
                String episodeName = saveTo + "\\" + "episode_" + (i + startSeed) + ".avi";
                encodeChunksToFile(fileNames[i], episodeName);
                progr += incPerEpisode;
                UpdateProgress((int)Math.Round(progr));
            }
            UpdateProgress(100);
            UpdateBtn("Download");
            Application.DoEvents();
        }

        private List<String> prepareRenTvEpisodesChunkUrls()
        {
            List<String> urls = new List<string>();
            if (String.IsNullOrEmpty(txtEpisodeUrls.Text)) return urls;
            foreach (var line in txtEpisodeUrls.Lines)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    String url = line.Replace("\\/", "/").Replace("playlist.m3u8", "chunklist.m3u8");
                    if (!url.StartsWith("http"))
                    {
                        url = "https://" + url;
                    }
                    urls.Add(url);
                }
            }
            return urls;
        }

        private List<String> prepareKaduRuSeriesUrls()
        {
            List<String> urls = new List<string>();
            if (String.IsNullOrEmpty(txtEpisodeUrls.Text)) return urls;
            foreach (var line in txtEpisodeUrls.Lines)
            {
                if (!String.IsNullOrEmpty(line))
                {
                    String url = line.Replace("\\/", "/");
                    if (!url.StartsWith("http"))
                    {
                        url = "http://" + url;
                    }
                    urls.Add(url);
                }
            }
            return urls;
        }

        private List<String> downloadAndProcessChunklist(String url)
        {
            List<String> names = new List<string>();
            using (var webClient = new WebClient())
            {
                String content = webClient.DownloadString(url);
                if (!String.IsNullOrEmpty(content))
                {
                    String[] lines = content.Split(new[] { "\n" } , StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (!line.StartsWith("#"))
                        {
                            names.Add(line);
                        }
                    }
                }
            }
            return names;
        }

        private List<String> downloadAndProcessKaduEpisodes(String url)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.LoadFromBrowser(url);
            var nodes = htmlDoc.DocumentNode.SelectNodes("//div[@id='video-line-content']/a");
            return nodes.Select(x => "http://kadu.ru" + x.Attributes["href"].Value).ToList();
        }

        private String downloadKaduVideoToFolder(String url, String saveToPath)
        {
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.LoadFromBrowser(url);
            var node = htmlDoc.DocumentNode.SelectSingleNode("//video[@id='video-flash']");
            var videoUrl = node.Attributes["src"].Value;
            var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//h1");
            var title = titleNode.InnerHtml;
            int t = title.IndexOf("</span>");
            title = title.Substring(t, title.Length - t) + ".mp4";
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(videoUrl, title);
            }
            return title;
        }

        private String downloadFileToFolder(String url, String saveToPath)
        {
            String fileName = Path.GetFileName(url);
            String fullFileName = saveToPath + "\\" + fileName;
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(url, fullFileName);
            }
            return fullFileName;
        }

        private void encodeChunksToFile(List<String> fileNames, String resultFileName)
        {
            String arguments = "";
            arguments += "-i \"concat:" + string.Join("|", fileNames);
            arguments += "\" -s 720x576 -c:v mpeg4 -vtag xvid -b:v 900k -c:a mp3 -ar 44100 -ac 2 " + resultFileName;
            var output = new StringBuilder();
            ProcessStartInfo encoder = new ProcessStartInfo();
            encoder.Arguments = arguments;
            encoder.FileName = ffmpegPath;
            encoder.WindowStyle = ProcessWindowStyle.Hidden;
            encoder.CreateNoWindow = true;
            encoder.RedirectStandardOutput = true;
            encoder.RedirectStandardError = true;
            encoder.UseShellExecute = false;
            using (Process proc = new Process())
            {
                proc.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
                proc.ErrorDataReceived += (sender, args) => output.AppendLine(args.Data);
                proc.StartInfo = encoder;

                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();
                proc.WaitForExit();
            }
        }

        private delegate void UpdateProgressDelegate(int progr);
        private void UpdateProgress(int progr)
        {
            if (this.progress.InvokeRequired)
            {
                this.Invoke(new UpdateProgressDelegate(this.UpdateProgress), new object[] { progr });
                return;
            }

            this.progress.Value = progr;
        }

        private delegate void UpdateBtnDelegate(String text);
        private void UpdateBtn(String text)
        {
            if (this.btnDownload.InvokeRequired)
            {
                this.Invoke(new UpdateBtnDelegate(this.UpdateBtn), new object[] { text });
                return;
            }

            this.btnDownload.Text = text;
        }
    }
}
