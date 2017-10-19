using Musiq.Providers.Player;
using System;
using System.Globalization;

namespace Musiq.UI
{
    public class Controls
    {
        private readonly Layout _layout;

        public Controls(Layout layout)
        {
            _layout = layout;
        }

        public void Display(MusiqPlayer player)
        {
            Console.SetCursorPosition(_layout.Width - 13, _layout.Height - 2);
            Console.Write($"Volume: {player.Volume.ToString("0%", new NumberFormatInfo { PercentSymbol = "%" })}");
        }
    }
}
