using System;

namespace visualsort
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new visualsort())
                game.Run();
        }
    }
}
