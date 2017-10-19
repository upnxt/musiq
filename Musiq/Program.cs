using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Musiq.Providers.Controls;
using System.Text;
using Musiq.UI;

namespace Musiq
{
    class Program
    {
        private static int _currentSongCursor;
        private static CancellationTokenSource _ts;

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;

            var ui = new Queue<Action>();

            Task.Run(() =>
            {
                while (true)
                {
                    if (ui.Count > 0)
                    {
                        var action = ui.Dequeue();
                        if (action != null)
                            action.Invoke();
                    }
                }
            });

            var window = new Window();
            var quit = false;

            using (var player = new Player(ui, new MusiqControl(), new ProgressBar(window), new TrackHud(window), new Settings(window)))
            {
                player.ListTracks(player.Playlist, 0);
                var selectedSongCursor = 0;

                _ts = new CancellationTokenSource();
                Task.Factory.StartNew(() => player.HUD(_ts), _ts.Token);

                while (!quit)
                {
                    var key = Console.ReadKey(true).Key;
                    switch (key)
                    {
                        case ConsoleKey.DownArrow:
                            if (selectedSongCursor >= player.Playlist.Count - 1)
                            {
                                selectedSongCursor = 0;
                            }
                            else
                                selectedSongCursor += 1;

                            player.ListTracks(player.Playlist, selectedSongCursor);
                            _currentSongCursor = player.Play(_currentSongCursor, selectedSongCursor);
                            break;

                        case ConsoleKey.UpArrow:
                            if (selectedSongCursor <= 0)
                            {
                                selectedSongCursor = 0;
                            }
                            else
                                selectedSongCursor -= 1;

                            player.ListTracks(player.Playlist, selectedSongCursor);
                            _currentSongCursor = player.Play(_currentSongCursor, selectedSongCursor);

                            break;

                        case ConsoleKey.P:

                            if (!player.IsPlaying)
                            {
                                _currentSongCursor = player.Play(_currentSongCursor, selectedSongCursor);
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

                        case ConsoleKey.O:
                            ui.Enqueue(() =>
                            {
                                player.Stop();

                                Console.Clear();
                                Console.SetCursorPosition(0, 0);
                                Console.Write("Add song directory: ");
                                var path = Console.ReadLine();

                                player.LoadPlaylist(path);
                                player.ListTracks(player.Playlist, selectedSongCursor);
                            });
                            break;

                        case ConsoleKey.Escape:
                        case ConsoleKey.Q:
                                player.Stop();
                                _ts.Cancel();
                                quit = true;

                                Console.Clear();
                                Console.SetCursorPosition(0, 0);
                                Console.Write("Quitting...");
                            break;
                    }
                }
            }
        }
    }
}