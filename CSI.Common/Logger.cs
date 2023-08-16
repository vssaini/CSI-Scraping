using System;

namespace CSI.Common
{
    public class Logger
    {
        public static void LogWithDoubleLines(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine("==========================================================================");
            Console.WriteLine(message);
            Console.WriteLine("==========================================================================");
            Console.WriteLine(Environment.NewLine);

            Console.ResetColor();
        }

        public static void LogInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"INFO: {message}");
            Console.ResetColor();
        }

        public static void LogSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"SUCCESS: {message}");
            Console.WriteLine(Environment.NewLine);
            Console.ResetColor();
        }

        public static void LogWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARN: {message}");
            Console.ResetColor();
        }

        public static void LogError(Exception exc)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine($"ERROR: {exc.Message}");
            Console.ResetColor();
        }

        public static void LogError(Exception exc, bool showStackTrace)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            if (showStackTrace)
            {
                Console.WriteLine(Environment.NewLine);
                Console.WriteLine($"ERROR: {exc}");
                Console.ResetColor();
                return;
            }

            LogError(exc);
        }
    }
}
