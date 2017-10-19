using System;

namespace Musiq.UI
{
    public class Window
    {
        public int Height => Console.WindowTop + Console.WindowHeight;
        public int Width => Console.WindowWidth;

        public void ResetCursor()
        {
            Console.SetCursorPosition(2, Height - 1);
        }
    }
}
