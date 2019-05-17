using System;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using YoutubeDownloader.Services;

namespace YoutubeDownloader
{
    public partial class Main : Form
    {
        private readonly string _apiKey = ConfigurationManager.AppSettings["ApiKey"];
        private readonly YouTubeClientWrapper youTubeClient;

        public Main()
        {
            InitializeComponent();
            youTubeClient = new YouTubeClientWrapper(_apiKey);
            this.SetDefaults();
        }

        private void SetDefaults()
        {
            lblDownloadPath.Text = System.IO.Path.GetTempPath();
            cmbFormat.SelectedIndex = 0;
        }

        private void btnDownloadPath_Click(object sender, EventArgs e)
        {
            DialogResult result = fbdPath.ShowDialog();

            if (result == DialogResult.OK)
            {
                lblDownloadPath.Text = fbdPath.SelectedPath;
            }
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            string id = null;
            string identifier = "videos";

            if (string.IsNullOrEmpty(txtUrl.Text) || 
                !HasId(ref id, ref identifier))
            {
                ShowErrorMessage();
                return;
            }

            youTubeClient.GetPlaylist(id, identifier, error => MessageBox.Show(error));
            youTubeClient.Format = cmbFormat.SelectedText;
            lstVideos.DataSource = youTubeClient.Videos.Select(SelectFunc).ToList();
            lblTotal.Text = youTubeClient.Videos.Count.ToString();
            btnDownload.Enabled = true;
        }

        private readonly Func<DTO.Item, string> SelectFunc = (item) =>
            Utilities.RemoveInvalidChars(item.Snippet.Title);

        private void ShowErrorMessage()
        {
            MessageBox.Show("Please enter a valid YouTube url");
            btnDownload.Enabled = false;
            if(lstVideos.Items.Count > 0)
            {
                lstVideos.Items.Clear();
            }
        }

        private bool HasId(ref string id, ref string identifier)
        {
            if (!Uri.IsWellFormedUriString(txtUrl.Text, UriKind.Absolute))
            {
                return false;
            }

            Uri unparsedUrl = new Uri(txtUrl.Text);
            var queryParams = HttpUtility.ParseQueryString(unparsedUrl.Query);

            if (queryParams.AllKeys.Contains("v"))
            {
                id = queryParams["v"];
                identifier = "videos";
                return true;
            }

            if (queryParams.AllKeys.Contains("list"))
            {
                id = queryParams["list"];
                identifier = "playlistItems";
                return true;
            }
            return false;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            var progressForm = new ProgressForm(youTubeClient, lblDownloadPath.Text);
            progressForm.Show();
        }

        private void cmbFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            youTubeClient.Format = cmbFormat.Text;
        }
    }
}
