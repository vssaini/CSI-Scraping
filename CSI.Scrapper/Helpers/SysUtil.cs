using System.Runtime.InteropServices;

namespace CSI.Scrapper.Helpers;

public class SysUtil
{
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern uint SetThreadExecutionState(uint esFlags);
}