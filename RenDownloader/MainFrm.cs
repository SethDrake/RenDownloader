using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace RenDownloader
{
    public partial class MainFrm : Form
    {
        private Thread downloadThread = null;

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
                UpdateBtn("Download");
                Application.DoEvents();
                return;
            }

            progress.Value = 0;
            UpdateBtn("Stop");
            Application.DoEvents();

            downloadThread = new Thread(new ThreadStart(downloadFiles));
            downloadThread.Start();
        }

        private void downloadFiles()
        {
            String saveTo = txtSavePath.Text;
            List<String> chunkUrls = prepareEpisodesChunkUrls(); 
            if (chunkUrls.Count == 0) return;
            int incPerChunk = 100 / chunkUrls.Count;
            float progr = 0;
            if (!Directory.Exists(saveTo))
            {
                Directory.CreateDirectory(saveTo);
            }
            for (int i = 0; i < chunkUrls.Count; i++)
            {
                String epDir = saveTo + "\\" + "episode" + (i + 1);
                if (!Directory.Exists(epDir))
                {
                    Directory.CreateDirectory(epDir);
                }
                List<String> fileNames = downloadAndProcessChunklist(chunkUrls[i]);
                float incPerFile = incPerChunk * 1.0f / fileNames.Count;
                foreach (var fileName in fileNames)
                {
                    downloadFileToFolder(chunkUrls[i].Replace("chunklist.m3u8", fileName), epDir);
                    progr += incPerFile;
                    UpdateProgress((int)Math.Round(progr));
                }
            }
            UpdateProgress(100);
            UpdateBtn("Download");
            Application.DoEvents();
        }

        private List<String> prepareEpisodesChunkUrls()
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

        private void downloadFileToFolder(String url, String saveToPath)
        {
            String fileName = Path.GetFileName(url);
            using (var webClient = new WebClient())
            {
                webClient.DownloadFile(url, saveToPath + "\\" + fileName);
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
