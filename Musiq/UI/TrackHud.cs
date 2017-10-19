using Musiq.Models;
using Musiq.Providers.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Musiq.UI
{
    public class TrackHud
    {
        private static object _lockObj = new object();
        private readonly Window _window;

        public TrackHud(Window window)
        {
            _window = window;
        }

        public void Display(MusiqControl player)
        {
            Console.SetCursorPosition(2, _window.Height - 4);

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

            Console.Write(player.Title.PadRight(_window.Width - 9));

            var trackMetaData = new StringBuilder();
            trackMetaData.Append($"{player.Artist}, ");

            if (!string.IsNullOrWhiteSpace(player.Album))
                trackMetaData.Append(player.Album);

            Console.SetCursorPosition(5, _window.Height - 3);
            Console.Write(trackMetaData.ToString().Trim(new[] { ',', ' ' }).PadRight(_window.Width - 10));

            Console.SetCursorPosition(_window.Width - 10, _window.Height - 4);
            Console.Write("{0:D2}:{1:D2}:{2:D2}", player.RemainingTime.Hours, player.RemainingTime.Minutes, player.RemainingTime.Seconds);
            _window.ResetCursor();
        }

        public int Play(MusiqControl player, List<Track> tracks, int currentSongCursor, int selectedSongCursor)
        {
            if (tracks?.Count <= 0)
                return 0;

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
