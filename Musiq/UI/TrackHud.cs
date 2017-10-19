using Musiq.Models;
using Musiq.Providers.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace Musiq.UI
{
    public class TrackHud
    {
        private static object _lockObj = new object();
        private readonly Layout _layout;

        public TrackHud(Layout layout)
        {
            _layout = layout;
        }

        public void Display(MusiqPlayer player)
        {
            Console.SetCursorPosition(2, _layout.Height - 3);

            if (player.IsPlaying)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{(char)9658}  "); //play
                Console.ResetColor();
            }
            else
            {
                Console.Write($"{(char)9612}{(char)9612} "); //pause
            }

            Console.Write(player.Title.PadRight(_layout.Width - 9));

            var trackMetaData = new StringBuilder();
            trackMetaData.Append($"{player.Artist}, ");

            if (!string.IsNullOrWhiteSpace(player.Album))
                trackMetaData.Append(player.Album);

            Console.SetCursorPosition(5, _layout.Height - 2);
            Console.Write(trackMetaData.ToString().Trim(new[] { ',', ' ' }).PadRight(_layout.Width - 10));

            Console.SetCursorPosition(_layout.Width - 10, _layout.Height - 3);
            Console.Write("{0:D2}:{1:D2}:{2:D2}", player.RemainingTime.Hours, player.RemainingTime.Minutes, player.RemainingTime.Seconds);
        }

        public int Play(MusiqPlayer player, List<Track> tracks, int currentSongCursor, int selectedSongCursor)
        {
            lock (_lockObj)
            {
                if (currentSongCursor != selectedSongCursor || player.Duration.TotalSeconds < 0)
                {
                    currentSongCursor = selectedSongCursor;
                    player.LoadAsync(tracks[currentSongCursor].FileName).Wait();
                }

                player.Play();
                return currentSongCursor;
            }
        }
    }
}
