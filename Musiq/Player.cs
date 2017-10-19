using Musiq.Models;
using Musiq.Providers.Library;
using Musiq.Providers.Controls;
using Musiq.UI;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Musiq
{
    public class Player : IMusiqControl, IDisposable
    {
        private readonly MusiqControl _control;
        private readonly ProgressBar _progressBar;
        private readonly TrackHud _trackHud;
        private readonly Settings _settings;

        private readonly MusiqLibrary _library;
        private readonly Queue<Action> _uiQueue;

        public Player(Queue<Action> uiQueue, MusiqControl control, ProgressBar progressBar, TrackHud trackHud, Settings settings)
        {
            _uiQueue = uiQueue;
            _control = control;
            _progressBar = progressBar;
            _trackHud = trackHud;
            _settings = settings;

            _library = new MusiqLibrary();
        }

        public List<Track> Playlist => _library.Playlist;

        #region Controls

        public bool IsPlaying => _control.IsPlaying;
        public bool Looping
        {
            get { return _control.Looping; }
            set { _control.Looping = value; }
        }

        public double Volume
        {
            get { return _control.Volume; }
            set { _control.Volume = value; }
        }

        public bool Play()
        {
            return _control.Play();
        }

        public bool Pause()
        {
            return _control.Pause();
        }

        public bool Stop()
        {
            return _control.Stop();
        }

        public void Loop()
        {
            _control.Loop();
        }

        public void FastForward()
        {
            _control.FastForward();
        }

        public void Rewind()
        {
            _control.Rewind();
        }

        public void Restart()
        {
            _control.Restart();
        }

        public void End()
        {
            _control.End();
        }

        #endregion Controls

        public void LoadPlaylist(params string[] paths)
        {
            _library.Find(paths);
        }

        public void ListTracks(List<Track> tracks, int selected)
        {
            _uiQueue.Enqueue(() =>
            {
                for (var i = 0; i < tracks.Count; i++)
                {
                    if (selected == i)
                        Console.BackgroundColor = ConsoleColor.DarkCyan;

                    Console.SetCursorPosition(0, i);
                    Console.WriteLine(tracks[i].FileName);
                    Console.ResetColor();
                }
            });
        }

        public int Play(int currentSongCursor, int selectedSongCursor)
        {
            return _trackHud.Play(_control, Playlist, currentSongCursor, selectedSongCursor);
        }

        public void HUD(CancellationTokenSource ts)
        {
            while (true)
            {
                _uiQueue.Enqueue(() =>
                {
                    _progressBar.Display(_control.Duration, _control.Position);
                    _trackHud.Display(_control);
                    _settings.Display(_control);
                });

                Thread.Sleep(200);

                if (ts.IsCancellationRequested)
                    break;
            }
        }

        public void Dispose()
        {
            _control.Dispose();
        }
    }
}
