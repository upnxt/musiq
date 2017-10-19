using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Musiq.Providers.Player;
using Musiq.Providers.Library;
using System.Text;
using Musiq.UI;
using Musiq.Models;
using System.IO;
using System.Reflection;

namespace Musiq
{
    class Program
    {
        private static object _lockObj = new object();

        private static int _currentSongCursor;
        private static CancellationTokenSource _ts;

        private static ProgressBar _progressBar;
        private static TrackHud _track;
        private static Controls _controls;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;

            var layout = new Layout();

            _progressBar = new ProgressBar(layout);
            _track = new TrackHud(layout);
            _controls = new Controls(layout);


            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), @"..\..\..\..\dependencies");
            var library = new MusiqLibrary();
            library.Find(path);

            using (var player = new MusiqPlayer())
            {
                ListTracks(library.Playlist, 0);
                var selectedSongCursor = 0;
                _ts = new CancellationTokenSource();
                Task.Factory.StartNew(() => PlayerHud(player, _ts), _ts.Token);

                do
                {
                    while (!Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).Key;
                        switch (key)
                        {
                            case ConsoleKey.DownArrow:
                                if (selectedSongCursor >= library.Playlist.Count - 1)
                                {
                                    selectedSongCursor = 0;
                                }
                                else
                                    selectedSongCursor += 1;

                                ListTracks(library.Playlist, selectedSongCursor);
                                _currentSongCursor = _track.Play(player, library.Playlist, _currentSongCursor, selectedSongCursor);
                                break;

                            case ConsoleKey.UpArrow:
                                if (selectedSongCursor <= 0)
                                {
                                    selectedSongCursor = 0;
                                }
                                else
                                    selectedSongCursor -= 1;

                                ListTracks(library.Playlist, selectedSongCursor);
                                _currentSongCursor = _track.Play(player, library.Playlist, _currentSongCursor, selectedSongCursor);

                                break;

                            case ConsoleKey.P:

                                if (!player.IsPlaying)
                                {
                                    _currentSongCursor = _track.Play(player, library.Playlist, _currentSongCursor, selectedSongCursor);
                                }
                                else
                                {
                                    player.Pause();
                                }

                                break;

                            case ConsoleKey.RightArrow:
                                player.FastForward();
                                break;

                            case ConsoleKey.LeftArrow:
                                player.Rewind();
                                break;

                            case ConsoleKey.Home:
                                player.Restart();
                                break;

                            case ConsoleKey.End:
                                player.End();
                                break;

                            case ConsoleKey.PageUp:
                                player.Volume += 1;
                                break;

                            case ConsoleKey.PageDown:
                                player.Volume -= 1;
                                break;
                        }
                    }
                } while (Console.ReadKey(true).Key != ConsoleKey.Escape);

                player.Stop();

                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
            }
        }

        private static void PlayerHud(MusiqPlayer player, CancellationTokenSource ts)
        {
            var height = Console.WindowTop + Console.WindowHeight - 5;

            while (true)
            {
                _progressBar.Display(player.Duration, player.Position);
                _track.Display(player);
                _controls.Display(player);

                Thread.Sleep(500);

                if (ts.IsCancellationRequested)
                    break;
            }
        }

        private static void ListTracks(List<Track> tracks, int selected)
        {
            for (var i = 0; i < tracks.Count; i++)
            {
                if (selected == i)
                    Console.BackgroundColor = ConsoleColor.DarkCyan;

                Console.SetCursorPosition(0, i);
                Console.WriteLine(tracks[i].FileName);
                Console.ResetColor();
            }
        }
    }
}