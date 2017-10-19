using Musiq.Models;
using System.IO;
using System.Text.RegularExpressions;

namespace Musiq.Handlers
{
    public class TrackMetaGenerator
    {
        public static Track Read(string filename)
        {
            var tag = new Track();

            if (string.IsNullOrWhiteSpace(filename))
                return tag;

            tag.Score += .25;
            tag.FileName = Path.GetFileName(filename);
            tag.FileType = Path.GetExtension(filename).Replace(".", "");

            var matches = Regex.Match(Path.GetFileNameWithoutExtension(filename), "(?<title>.*?)[-](?<artist>.*)");
            if (matches.Groups["title"] != null && !string.IsNullOrWhiteSpace(matches.Groups["title"].Value))
            {
                tag.Title = matches.Groups["title"].Value.Trim();
                tag.Score += .5;
            }
            else
            {
                tag.Title = Path.GetFileNameWithoutExtension(filename);
                tag.Score += .25;
            }

            if (matches.Groups["artist"] != null && !string.IsNullOrWhiteSpace(matches.Groups["artist"].Value))
            {
                tag.Artist = matches.Groups["artist"].Value.Trim();
                tag.Score += .25;
            }

            return tag;
        }
    }
}
