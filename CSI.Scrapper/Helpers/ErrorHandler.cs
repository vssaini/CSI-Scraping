using CSI.Scrapper.Properties;
using System;
using System.Threading;
using System.Windows.Forms;

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
            MessageBox.Show(Resources.CrashProgramError + " " + e, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CrashHandler_thread(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(Resources.CrashThreadError + " " + e, Resources.MsgBoxErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
