using Musiq.Providers.Controls;
using System;
using System.Globalization;

namespace Musiq.UI
{
    public class Settings
    {
        private readonly Window _window;

        public Settings(Window window)
        {
            _window = window;
        }

        public void Display(MusiqControl player)
        {
            Console.SetCursorPosition(_window.Width - 13, _window.Height - 3);
            Console.Write($"Volume: {player.Volume.ToString("0%", new NumberFormatInfo { PercentSymbol = "%" })}");
            _window.ResetCursor();
        }
    }
}
