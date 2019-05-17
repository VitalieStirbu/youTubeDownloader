using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RestSharp;
using YoutubeDownloader.DTO;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace YoutubeDownloader.Services
{
    public class YouTubeClientWrapper
    {
        private string apiKey;
        public List<Item> Videos { get; set; }

        public string ApiKey
        {
            get => apiKey;
        }

        public string Format { get; set; }

        public YouTubeClientWrapper(string apiKey)
        {
            this.apiKey = apiKey;

            Videos = new List<Item>();
        }

        public void GetPlaylist(string id, string identifier, Action<string> errorHandler = null)
        {
            Videos.Clear();

            PlayListResult response = GetDataAsync(id, identifier, null, errorHandler);
            Videos.AddRange(response.Items);

            while (response.NextPageToken != null)
            {
                response = GetDataAsync(id, identifier, response.NextPageToken, errorHandler);
                Videos.AddRange(response.Items);
            }
        }

        private PlayListResult GetDataAsync(string id, string identifier, string pageToken = null, Action<string> ErrorHandler = null)
        {
            PlayListResult result = null;

            var client = new RestClient("https://www.googleapis.com");

            var request = new RestRequest($"youtube/v3/{identifier}", Method.GET);
            request.AddParameter("part", "snippet");
            request.AddParameter("maxResults", "50");
            request.AddParameter(identifier == "playlistItems" ? "playlistId" : "id", id);
            request.AddParameter("key", this.apiKey);

            if (pageToken != null)
            {
                request.AddParameter("pageToken", pageToken);
            }

            try
            {
                var req = client.Execute<PlayListResult>(request);
                result = req.Data;
            }
            catch (Exception e)
            {
                ErrorHandler?.Invoke(e.Message);
            }

            return result;
        }

        public async Task DownloadVideos(string path, Action<int> indexChanged)
        {
            var client = new YoutubeClient();

            for (var index = 0; index < Videos.Count; index++)
            {
                Item video = Videos[index];
                // Get metadata for all streams in this video

                var streamInfoSet = video.Snippet.ResourceId != null
                    ? await client.GetVideoMediaStreamInfosAsync(video.Snippet.ResourceId.VideoId)
                    : await client.GetVideoMediaStreamInfosAsync(video.Id);

                var streamInfo = streamInfoSet.Audio.WithHighestBitrate();

                // Get file extension based on stream's container
                var ext = streamInfo.Container.GetFileExtension();

                string fileName = Utilities.RemoveInvalidChars(video.Snippet.Title);
                string extension = Format == "Default" ? ext : Format;
                string fullPath = System.IO.Path.Combine(path, $"{fileName}.{extension}");

                // Download stream to file
                await client.DownloadMediaStreamAsync(streamInfo, fullPath);

                indexChanged(index + 1);
            }
        }
    }
}
