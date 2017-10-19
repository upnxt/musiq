using ManagedBass;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;


namespace Musiq.Providers.Player
{
    public abstract class PlayerBase : INotifyPropertyChanged, IDisposable
    {
        #region Fields
        readonly SynchronizationContext _syncContext;
        int _handle;

        /// <summary>
        /// Channel Handle of the loaded audio file.
        /// </summary>
        protected internal int Handle
        {
            get { return _handle; }
            private set
            {
                if (!Bass.ChannelGetInfo(value, out var info))
                    throw new ArgumentException("Invalid Channel Handle: " + value);

                _handle = value;

                // Init Events
                Bass.ChannelSetSync(Handle, SyncFlags.Free, 0, GetSyncProcedure(() => Disposed?.Invoke(this, EventArgs.Empty)));
                Bass.ChannelSetSync(Handle, SyncFlags.Stop, 0, GetSyncProcedure(() => MediaFailed?.Invoke(this, EventArgs.Empty)));
                Bass.ChannelSetSync(Handle, SyncFlags.End, 0, GetSyncProcedure(() =>
                {
                    try
                    {
                        if (!Bass.ChannelHasFlag(Handle, BassFlags.Loop))
                            MediaEnded?.Invoke(this, EventArgs.Empty);
                    }
                    finally { OnStateChanged(); }
                }));
            }
        }

        protected bool _restartOnNextPlayback;
        #endregion

        SyncProcedure GetSyncProcedure(Action Handler)
        {
            return (SyncHandle, Channel, Data, User) =>
            {
                if (Handler == null)
                    return;

                if (_syncContext == null)
                    Handler();
                else _syncContext.Post(S => Handler(), null);
            };
        }

        static PlayerBase()
        {
            var currentDev = Bass.CurrentDevice;

            if (currentDev == -1 || !Bass.GetDeviceInfo(Bass.CurrentDevice).IsInitialized)
                Bass.Init(currentDev);
        }

        public PlayerBase()
        {
            _syncContext = SynchronizationContext.Current;
        }

        #region Events

        /// <summary>
        /// Fired when a Media is Loaded.
        /// </summary>
        public event Action<int, string> MediaLoaded;

        /// <summary>
        /// Fired when the Media Playback Ends
        /// </summary>
        public event EventHandler MediaEnded;

        /// <summary>
        /// Fired when the Playback fails
        /// </summary>
        public event EventHandler MediaFailed;

        /// <summary>
        /// Fired when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Fired when this Channel is Disposed.
        /// </summary>
        public event EventHandler Disposed;

        #endregion

        #region Frequency

        double _freq = 44100;

        /// <summary>
        /// Gets or Sets the Playback Frequency in Hertz.
        /// Default is 44100 Hz.
        /// </summary>
        public double Frequency
        {
            get { return _freq; }
            set
            {
                if (!Bass.ChannelSetAttribute(Handle, ChannelAttribute.Frequency, value))
                    return;

                _freq = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Balance

        double _pan;

        /// <summary>
        /// Gets or Sets Balance (Panning) (-1 ... 0 ... 1).
        /// -1 Represents Completely Left.
        ///  1 Represents Completely Right.
        /// Default is 0.
        /// </summary>
        public double Balance
        {
            get { return _pan; }
            set
            {
                if (!Bass.ChannelSetAttribute(Handle, ChannelAttribute.Pan, value))
                    return;

                _pan = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Device

        int _dev = -1;

        /// <summary>
        /// Gets or Sets the Playback Device used.
        /// </summary>
        public int Device
        {
            get { return (_dev = _dev == -1 ? Bass.ChannelGetDevice(Handle) : _dev); }
            set
            {
                if (!Bass.GetDeviceInfo(value).IsInitialized)
                    if (!Bass.Init(value))
                        return;

                if (!Bass.ChannelSetDevice(Handle, value))
                    return;

                _dev = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Volume

        double _vol = 0.5;

        /// <summary>
        /// Gets or Sets the Playback Volume.
        /// </summary>
        public double Volume
        {
            get { return _vol; }
            set
            {
                if (!Bass.ChannelSetAttribute(Handle, ChannelAttribute.Volume, value))
                    return;

                _vol = value;
                OnPropertyChanged();
            }
        }

        #endregion

        /// <summary>
        /// Override this method for custom loading procedure.
        /// </summary>
        /// <param name="FileName">Path to the File to Load.</param>
        /// <returns><see langword="true"/> on Success, <see langword="false"/> on failure</returns>
        protected virtual int OnLoad(string FileName) => Bass.CreateStream(FileName);

        /// <summary>
        /// Gets the Playback State of the Channel.
        /// </summary>
        public PlaybackState State => Handle == 0 ? PlaybackState.Stopped : Bass.ChannelIsActive(Handle);
        
        /// <summary>
        /// Loads a file into the player.
        /// </summary>
        /// <param name="FileName">Path to the file to Load.</param>
        /// <returns><see langword="true"/> on succes, <see langword="false"/> on failure.</returns>
        public async Task<bool> LoadAsync(string FileName)
        {
            try
            {
                if (Handle != 0)
                    Bass.StreamFree(Handle);
            }
            catch { }

            if (_dev != -1)
                Bass.CurrentDevice = _dev;

            var currentDev = Bass.CurrentDevice;

            if (currentDev == -1 || !Bass.GetDeviceInfo(Bass.CurrentDevice).IsInitialized)
                Bass.Init(currentDev);

            var h = await Task.Run(() => OnLoad(FileName));
            if (h == 0)
                return false;

            Handle = h;
            InitProperties();
            MediaLoaded?.Invoke(h, FileName);
            OnPropertyChanged("");

            return true;
        }

        /// <summary>
        /// Frees all resources used by the player.
        /// </summary>
        public virtual void Dispose()
        {
            try
            {
                if (Bass.StreamFree(Handle))
                    _handle = 0;
            }
            finally { OnStateChanged(); }
        }

        /// <summary>
        /// Initializes Properties on every call to <see cref="LoadAsync"/>.
        /// </summary>
        protected virtual void InitProperties()
        {
            Frequency = _freq;
            Balance = _pan;
            Volume = _vol;
        }

        protected void OnStateChanged() => OnPropertyChanged(nameof(State));

        /// <summary>
        /// Fires the <see cref="PropertyChanged"/> event.
        /// </summary>
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            if (PropertyChanged == null)
                return;

            Action f = () => PropertyChanged.Invoke(this, new PropertyChangedEventArgs(PropertyName));

            if (_syncContext == null)
                f();
            else _syncContext.Post(S => f(), null);
        }
    }
}
