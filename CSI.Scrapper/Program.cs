using CSI.Scrapper.Helpers;
using Serilog;
using System;
using System.Windows.Forms;

namespace CSI.Scrapper
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Startup.ConfigureLogger();
            Log.Information("Starting application CSI Scrapper");

            ErrorHandler.ConfigureGlobalErrorHandling();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
