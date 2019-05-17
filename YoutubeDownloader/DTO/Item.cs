using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeDownloader.DTO
{
    public class Item
    {
        public string Id { get; set; }
        public string Kind { get; set; }
        public string Etag { get; set; }
        public Snippet Snippet { get; set; }
    }
}
