using Musiq.Models;
using System.Collections.Generic;
using System.IO;

namespace Musiq.Providers.Library
{
    public class MusiqLibrary
    {
        public MusiqLibrary()
        {
            Playlist = new List<Track>();
        }

        public List<Track> Playlist { get; }

        public void Find(params string[] paths)
        {
            foreach (var path in paths)
            {
                var files = Directory.GetFiles(path, "*.mp3");
                foreach (var file in files)
                    Playlist.Add(new Track { FileName = file });
            }
        }
    }
}
