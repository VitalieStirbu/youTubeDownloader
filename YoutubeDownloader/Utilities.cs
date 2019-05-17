using System.IO;
using System.Text.RegularExpressions;

namespace YoutubeDownloader
{
    public static class Utilities
    {
        public static string RemoveInvalidChars(string value)
        {
            string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return r.Replace(value, "");
        }
    }
}
