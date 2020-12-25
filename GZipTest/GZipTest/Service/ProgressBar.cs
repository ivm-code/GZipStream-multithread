using System;

namespace GZipTest
{
    public class ProgressBar : IProgress
    {
        public void ShowProgress(long current, long whole)
        {
            Console.Write(string.Format("Processed {0:p}", (float)current / whole));
            Console.SetCursorPosition(0, Console.CursorTop);
        }
    }
}