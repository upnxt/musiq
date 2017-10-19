using System;

namespace Musiq.UI
{
    public class ProgressBar
    {
        private readonly Window _window;

        public ProgressBar(Window window)
        {
            _window = window;
        }

        public void Display(TimeSpan duration, TimeSpan position)
        {
            for (var i = 0; i < _window.Width; i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                var percentRemaining = position.TotalSeconds /duration.TotalSeconds;
                if ((double)i / (double)_window.Width >= percentRemaining)
                    Console.ResetColor();

                Console.SetCursorPosition(i, (_window.Height - 6));
                Console.Write("=");
            }

            Console.ResetColor();
            _window.ResetCursor();
        }
    }
}
