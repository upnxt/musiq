namespace Musiq.Providers.Controls
{
    public interface IMusiqControl
    {
        bool Play();
        bool IsPlaying { get; }
        bool Pause();
        bool Stop();
        bool Looping { get; set; }
        void Loop();
        void FastForward();
        void Rewind();
        void Restart();
        void End();
        double Volume { get; set; }
    }
}
