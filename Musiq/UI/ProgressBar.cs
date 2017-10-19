using System;

namespace Musiq.UI
{
    public class ProgressBar
    {
        private readonly Layout _layout;

        public ProgressBar(Layout layout)
        {
            _layout = layout;
        }

        public void Display(TimeSpan duration, TimeSpan position)
        {
            for (var i = 0; i < _layout.Width; i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                var percentRemaining = position.TotalSeconds /duration.TotalSeconds;
                if ((double)i / (double)_layout.Width >= percentRemaining)
                    Console.ResetColor();

                Console.SetCursorPosition(i, (_layout.Height - 5));
                Console.Write("=");
            }

            Console.ResetColor();
        }
    }
}
