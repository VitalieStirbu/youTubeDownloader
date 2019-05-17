using System;

namespace YoutubeDownloader.DTO
{
    public class Snippet    
    {
        public DateTime PublishedAt { get; set; }
        public string ChannelId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ChannelTitle { get; set; }
        public string PlaylistId { get; set; }
        public int Position { get; set; }
        public Thumbnails Thumbnails { get; set; }
        public ResourceId ResourceId { get; set; }
    }
}