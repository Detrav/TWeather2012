#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TWeather2012
{
#if WINDOWS
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (MainClass game = new MainClass())
            {
                game.Run();
            }
        }
    }
#endif
}
