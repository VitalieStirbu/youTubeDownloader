using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.DTO
{
    public class PlayListResult
    {
        public string Kind { get; set; }
        public string Etag { get; set; }
        public string NextPageToken { get; set; }
        public PageInfo PageInfo { get; set; }
        public List<Item> Items { get; set; }
    }
}
