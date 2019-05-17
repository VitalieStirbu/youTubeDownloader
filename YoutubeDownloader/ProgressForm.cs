using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeDownloader.DTO;
using YoutubeDownloader.Services;

namespace YoutubeDownloader
{
    public partial class ProgressForm : Form
    {
        private YouTubeClientWrapper youTubeClient;
        private string lblDownloadPath;

        public ProgressForm()
        {
            InitializeComponent();
        }

        public ProgressForm(YouTubeClientWrapper youTubeClient, string lblDownloadPath)
        {
            InitializeComponent();
            this.youTubeClient = youTubeClient;
            this.lblDownloadPath = lblDownloadPath;
            pgbDownloads.Maximum = 100;
            this.Text = youTubeClient.Videos[0].Snippet.Title; 
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            youTubeClient.DownloadVideos(this.lblDownloadPath, index =>
            {
                backgroundWorker1.ReportProgress(index);  
            }).Wait();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int index = e.ProgressPercentage;
            decimal percentage = 100 * index / youTubeClient.Videos.Count;
            percentage = Math.Floor(percentage);
            
            // Change the value of the ProgressBar   
            pgbDownloads.Value = (int)percentage;  
           
            // Set the text.  
            if (index < youTubeClient.Videos.Count)
            {
                this.Text = youTubeClient.Videos[index].Snippet.Title; 
            }
        }

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerAsync();  
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("Success!");
            this.Close();
        }
    }
}
