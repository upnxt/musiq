using ManagedBass;
using Musiq.Handlers;
using System;

namespace Musiq.Providers.Controls
{
    public class MusiqControl : ControlBase, IMusiqControl
    {
        public MusiqControl()
        {
            MediaLoaded += MusiqPlayer_MediaLoaded;
        }

        #region Events

        private void MusiqPlayer_MediaLoaded(int handle, string filename)
        {
            var embededTags = TagReader.Read(handle);
            var parsedTags = TrackMetaGenerator.Read(filename);

            Title = !string.IsNullOrWhiteSpace(embededTags.Title) ? embededTags.Title : parsedTags.Title;
            Artist = !string.IsNullOrWhiteSpace(embededTags.Artist) ? embededTags.Artist : parsedTags.Artist;
            Album = !string.IsNullOrWhiteSpace(embededTags.Album) ? embededTags.Album : parsedTags.Album;
        }

        #endregion Events

        #region Playback

        public bool Play()
        {
            try
            {
                var result = Bass.ChannelPlay(Handle, _restartOnNextPlayback);

                if (result)
                    _restartOnNextPlayback = false;

                IsPlaying = true;
                return result;
            }
            finally { OnStateChanged(); }
        }

        public bool IsPlaying { get; private set; }

        public bool Pause()
        {
            try
            {
                IsPlaying = false;
                return Bass.ChannelPause(Handle);
            }
            finally { OnStateChanged(); }
        }

        public bool Stop()
        {
            try
            {
                IsPlaying = false;
                _restartOnNextPlayback = true;
                return Bass.ChannelStop(Handle);
            }
            finally { OnStateChanged(); }
        }

        public bool Looping { get; set; }

        public void Loop()
        {
            try
            {
                if (!Looping)
                {
                    Looping = true;
                    Bass.ChannelAddFlag(Handle, BassFlags.Loop);
                }
                else
                {
                    Looping = false;
                    Bass.ChannelRemoveFlag(Handle, BassFlags.Loop);
                }
            }
            finally { OnPropertyChanged(); }
        }
        
        public void FastForward()
        {
            try
            {
                Bass.ChannelSetPosition(Handle, Bass.ChannelSeconds2Bytes(Handle, Position.TotalSeconds + 1));
            }
            finally { OnStateChanged(); }
        }

        public void Rewind()
        {
            try
            {
                var position = Bass.ChannelGetPosition(Handle);
                Bass.ChannelSetPosition(Handle, Bass.ChannelSeconds2Bytes(Handle, Position.TotalSeconds - 1));
            }
            finally { OnStateChanged(); }
        }

        public void Restart()
        {
            try
            {
                Bass.ChannelSetPosition(Handle, Bass.ChannelSeconds2Bytes(Handle, 0));
            }
            finally { OnStateChanged(); }
        }

        public void End()
        {
            try
            {
                Bass.ChannelSetPosition(Handle, Bass.ChannelSeconds2Bytes(Handle, Duration.TotalSeconds - 5));
            }
            finally { OnStateChanged(); }
        }

        #endregion Playback

        #region Track

        public TimeSpan Duration => TimeSpan.FromSeconds(Bass.ChannelBytes2Seconds(Handle, Bass.ChannelGetLength(Handle)));

        public TimeSpan Position
        {
            get { return TimeSpan.FromSeconds(Bass.ChannelBytes2Seconds(Handle, Bass.ChannelGetPosition(Handle))); }
            set { Bass.ChannelSetPosition(Handle, Bass.ChannelSeconds2Bytes(Handle, value.TotalSeconds)); }
        }

        public TimeSpan RemainingTime => (Duration - Position);

        private string _title = string.Empty;
        public string Title
        {
            get { return _title; }
            private set
            {
                _title = value;
                OnPropertyChanged();
            }
        }

        private string _artist = string.Empty;
        public string Artist
        {
            get { return _artist; }
            private set
            {
                _artist = value;
                OnPropertyChanged();
            }
        }

        private string _album = string.Empty;
        public string Album
        {
            get { return _album; }
            private set
            {
                _album = value;
                OnPropertyChanged();
            }
        }

        #endregion Track
    }
}
