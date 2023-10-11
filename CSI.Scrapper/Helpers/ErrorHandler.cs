using CSI.Scrapper.Properties;
using System;
using System.Threading;
using System.Windows.Forms;
using Serilog;

namespace CSI.Scrapper.Helpers
{
    internal class ErrorHandler
    {
        public static void ConfigureGlobalErrorHandling()
        {
            // Error handling for application
            var currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CrashHandler;
            Application.ThreadException += CrashHandler_thread;
        }

        private static void CrashHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Logger.Error("Unhandled error: " + e.ExceptionObject);

            MessageBox.Show(Resources.CrashProgramError + " " + e, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CrashHandler_thread(object sender, ThreadExceptionEventArgs e)
        {
            Log.Logger.Error(e.Exception, "Thread error.");

            MessageBox.Show(Resources.CrashThreadError + " " + e, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
